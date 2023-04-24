﻿// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;
using OmniCore.Framework.Omnipod;
using OmniCore.Framework.Omnipod.Messages;
using OmniCore.Framework.Omnipod.Requests;
using OmniCore.Services.Interfaces.Pod;
//using Moq;
//using OmniCore.Common.Data;
//using OmniCore.Framework.Omnipod.Messages;
//using OmniCore.Services;
//using OmniCore.Services.Interfaces.Core;
//using OmniCore.Services.Interfaces.Pod;
//using OmniCore.Tests;
//using Plugin.BLE.Abstractions.Extensions;



var message = new MessageBuilder()
    .WithSequence(0)
    .WithAddress(0x01020304)
    .Build(new SetBeepingMessage
    {
        BeepNow = BeepType.BeepBeep
    });

Console.WriteLine(JsonSerializer.Serialize(message));

var message2 = new MessageBuilder().Build(message.Body);

Console.WriteLine(JsonSerializer.Serialize(message2));


// {"MyProperty1":99,"MyProperty2":9999}

// var dataService = new Mock<IDataService>().Object;
// var pod = new Pod(dataService)
// {
//     ValidFrom = DateTimeOffset.Now,
//     ValidTo = DateTimeOffset.Now + TimeSpan.FromHours(80),
//     Medication = MedicationType.Insulin,
//     RadioAddress = 0x55555555,
//     UnitsPerMilliliter = 100,
//     Progress = PodProgress.Running
// };
// var radioConnection = new MockRadioConnection(pod);
//
// var podLock = await pod.LockAsync(CancellationToken.None);
// using (var podConn = new PodConnection(pod, radioConnection, podLock, dataService))
// {
//     var result = await podConn.SetTempBasal(0, 8);
//     Console.WriteLine($"{result}");
// }

// PodMessage ConstructMessage(bool critical, MessagePart[] parts)
// {
//     var msgParts = new List<IMessagePart>();
//     foreach (var part in parts)
//     {
//         if (part.RequiresNonce)
//             part.Nonce = 0x12345678;
//         msgParts.Add(part);
//     }
//
//     return new PodMessage
//     {
//         Address = 0x34343434,
//         Sequence = 0,
//         WithCriticalFollowup = critical,
//         Parts = msgParts
//     };
// }
