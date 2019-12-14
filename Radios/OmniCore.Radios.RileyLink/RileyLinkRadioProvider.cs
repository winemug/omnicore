﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OmniCore.Model.Constants;
using OmniCore.Model.Enumerations;
using OmniCore.Model.Interfaces;
using OmniCore.Model.Interfaces.Entities;
using OmniCore.Model.Interfaces.Platform;
using OmniCore.Model.Interfaces.Repositories;
using OmniCore.Model.Interfaces.Workflow;
using Unity;


namespace OmniCore.Radios.RileyLink
{
    public class RileyLinkRadioProvider : IRadioProvider
    {
        private readonly Guid RileyLinkServiceUUID = Guid.Parse("0235733b-99c5-4197-b856-69219c2a3845");

        private readonly IRadioAdapter RadioAdapter;
        private readonly IRadioRepository RadioRepository;
        private readonly ISignalStrengthRepository SignalStrengthRepository;
        private readonly IUnityContainer Container;

        private readonly AsyncLock RadioDictionaryLock;
        private readonly Dictionary<Guid,IRadio> RadioDictionary;
        private readonly IRadioEventRepository RadioEventRepository;

        public RileyLinkRadioProvider(
            IRadioAdapter radioAdapter, 
            IRadioRepository radioRepository,
            ISignalStrengthRepository signalStrengthRepository,
            IRadioEventRepository radioEventRepository,
            IUnityContainer container)
        {
            RadioAdapter = radioAdapter;
            RadioRepository = radioRepository;
            SignalStrengthRepository = signalStrengthRepository;
            RadioEventRepository = radioEventRepository;
            Container = container;
            RadioDictionary = new Dictionary<Guid, IRadio>();
            RadioDictionaryLock = new AsyncLock();
        }

        public string Description => "RileyLink";
       
        public IObservable<IRadio> ListRadios()
        {
            return Observable.Create<IRadio>( (IObserver<IRadio> observer) =>
            {
                var cts = new CancellationTokenSource();
                
                var scanner = RadioAdapter.FindPeripherals(RileyLinkServiceUUID)
                    .Subscribe(async peripheralResult =>
                    {
                        var radio = await GetRadio(peripheralResult.Peripheral, cts.Token);

                        if (peripheralResult.Rssi.HasValue)
                        {
                            var sse = SignalStrengthRepository.New();
                            sse.Radio = radio.Entity;
                            sse.Rssi = peripheralResult.Rssi.Value;
                            await SignalStrengthRepository.Create(sse, cts.Token);
                        }

                        var radioEvent = RadioEventRepository.New();
                        radioEvent.Radio = radio.Entity;
                        radioEvent.EventType = RadioEvent.Scan;
                        radioEvent.Success = true;
                        await RadioEventRepository.Create(radioEvent, cts.Token);

                        observer.OnNext(radio);
                    });
                
                return Disposable.Create(() =>
                {
                    cts.Cancel();
                    cts.Dispose();
                    scanner.Dispose();
                });
            });
        }

        private async Task<IRadio> GetRadio(IRadioPeripheral peripheral, CancellationToken cancellationToken)
        {
            using var lockObj = await RadioDictionaryLock.LockAsync();
            IRadio radio = null;
            
            if (RadioDictionary.ContainsKey(peripheral.PeripheralUuid))
            {
                radio = RadioDictionary[peripheral.PeripheralUuid];
            }
            else
            {
                var entity = await RadioRepository.ByDeviceUuid(peripheral.PeripheralUuid);
                if (entity == null)
                {
                    entity = RadioRepository.New();
                    entity.DeviceUuid = peripheral.PeripheralUuid;
                    entity.DeviceName = peripheral.PeripheralName;
                    await RadioRepository.Create(entity, cancellationToken);
                }

                radio = Container.Resolve<IRadio>(RegistrationConstants.RileyLink);
                radio.Entity = entity;
                RadioDictionary.Add(peripheral.PeripheralUuid, radio);
            }
            radio.Peripheral = peripheral;
            return radio;
        }
    }
}