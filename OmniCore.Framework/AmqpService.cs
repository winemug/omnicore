using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel.Resolution;
using Nito.AsyncEx;
using OmniCore.Common.Amqp;
using OmniCore.Common.Api;
using OmniCore.Services.Interfaces;
using OmniCore.Services.Interfaces.Amqp;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Entities;
using OmniCore.Services.Interfaces.Platform;
using OmniCore.Shared.Api;
using Polly;
using Polly.Timeout;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OmniCore.Services;

public class AmqpService : IAmqpService
{
    private Task _amqpTask;
    private CancellationTokenSource _cts;
    private readonly X509Certificate2Collection _caCertificateCollection;

    private readonly AsyncProducerConsumerQueue<AmqpMessage> _publishQueue;
    private readonly AsyncProducerConsumerQueue<AmqpMessage> _confirmQueue;

    private IAppConfiguration _appConfiguration;
    private IApiClient _apiClient;
    private IPlatformInfo _platformInfo;

    public AmqpService(IAppConfiguration appConfiguration,
        IApiClient apiClient,
        IPlatformInfo platformInfo)
    {
        _appConfiguration = appConfiguration;
        _apiClient = apiClient;
        _platformInfo = platformInfo;

        _publishQueue = new AsyncProducerConsumerQueue<AmqpMessage>();

        _caCertificateCollection = new X509Certificate2Collection
        {
            X509Certificate2.CreateFromPem(ApiConstants.RootCertificate),
            X509Certificate2.CreateFromPem(ApiConstants.CaCertificate)
        };
    }

    public async Task Start()
    {
        _cts = new CancellationTokenSource();
        _amqpTask = Task.Run(async () => await ServiceTask(_cts.Token));
    }

    public async Task Stop()
    {
        try
        {
            await _cts.CancelAsync();
            await _amqpTask;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error while cancelling core task: {e}");
        }
        finally
        {
            foreach (var caCert in _caCertificateCollection)
            {
                caCert.Dispose();
            }
        }
    }

    public async Task PublishMessage(AmqpMessage message)
    {
        try
        {
            await _publishQueue.EnqueueAsync(message);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"Error enqueuing message {e}");
            throw;
        }
    }

    private async Task ServiceTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var configuration = await _appConfiguration.Get();
            while (configuration == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                configuration = await _appConfiguration.Get();
            }

            try
            {
                await AmqpSequence(configuration, cancellationToken);
            }
            catch (Exception e)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Trace.WriteLine($"Error while connectandpublish: {e.Message}");
                await Task.Delay(15000);
            }
        }
    }

    private async Task<IConnection> TryCreateConnection(string amqpConnectionString, X509Certificate2 clientCertificate,
        CancellationToken cancellationToken = default)
    {
        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(amqpConnectionString),
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            AutomaticRecoveryEnabled = false,
            AuthMechanisms = new IAuthMechanismFactory[] { new ExternalMechanismFactory() },
        };

        connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _) => clientCertificate;
        connectionFactory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors;
        connectionFactory.Ssl.CertificateValidationCallback = (_, _, _, _) => true;

        return Policy<IConnection>
            .Handle<Exception>()
            .WaitAndRetryForever(
                sleepDurationProvider: retries => { return TimeSpan.FromSeconds(Math.Min(retries * 3, 60)); },
                onRetry: (ex, ts) => { Trace.WriteLine($"Error {ex}, waiting {ts} to reconnect"); })
            .Execute((_) => connectionFactory.CreateConnection(), cancellationToken);
    }

    private async Task<IModel> CreateSubscription(string queue, string exchange, string routingKey,
        IConnection connection, AmqpDestination destination, CancellationToken cancellationToken = default)
    {
        using (var cmdChannel = connection.CreateModel())
        {
            cmdChannel.QueueDeclare(queue, true, false, false);
            cmdChannel.ExchangeDeclare(exchange, "direct", true, false);
            cmdChannel.QueueBind(queue, exchange, routingKey);
        }
        
        var subChannel = connection.CreateModel();
        var consumer = new AsyncEventingBasicConsumer(subChannel);
        consumer.Received += async (sender, ea) =>
            await ProcessMessage(ea, subChannel, destination);
        subChannel.BasicConsume(queue, false, consumer);
        return subChannel;
    }

    private async Task AmqpMessageLoop(
        IConnection connection,
        IModel pubChannel,
        OmniCoreConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var confirmList = new List<AmqpMessage>();

        try
        {
            while (!cancellationToken.IsCancellationRequested && connection.IsOpen)
            {
                AmqpMessage? publishMessage = null;
                try
                {
                    publishMessage = await Policy.TimeoutAsync(15)
                        .ExecuteAsync((t) => _publishQueue.DequeueAsync(t), cancellationToken);
                }
                catch (TimeoutRejectedException)
                {
                }
            
                if (publishMessage != null)
                {
                    confirmList.Add(publishMessage);
                    var properties = pubChannel.CreateBasicProperties();
                    properties.UserId = configuration.UserId;
                    properties.Type = publishMessage.Type;
                    pubChannel.BasicPublish(publishMessage.Exchange, publishMessage.Route, false,
                        properties, Encoding.UTF8.GetBytes(publishMessage.Text));
                    continue;
                }
            
                pubChannel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(30));
                
                if (confirmList.Count > 0)
                {
                    var confirmTasks = confirmList.Where(m => m.OnPublishConfirmed != null)
                        .Select(m => (Task)m.OnPublishConfirmed).ToList();
                    confirmList = new List<AmqpMessage>();
                    Task.Run(async () =>
                    {
                        foreach(var task in confirmTasks)
                            try
                            {
                                await task;
                            }
                            catch
                            {
                            }
                    });
                }
            }
        }
        finally
        {
            foreach (var msg in confirmList)
                await _publishQueue.EnqueueAsync(msg);
        }
    }
    private async Task AmqpSequence(OmniCoreConfiguration configuration, CancellationToken cancellationToken)
    {
        using var certWithKey = X509Certificate2.CreateFromPem(configuration.ClientCertificate, configuration.ClientKey);
        var data = certWithKey.Export(X509ContentType.Pkcs12);
        using var clientCertificate = new X509Certificate2(data, "", X509KeyStorageFlags.DefaultKeySet);

        using var connection =
            await TryCreateConnection(configuration.AmqpConnectionString, clientCertificate, cancellationToken);

        using var subChannel1 = await CreateSubscription($"q_req_{configuration.UserId}", configuration.RequestExchange,
            configuration.UserId, connection, AmqpDestination.Request, cancellationToken);

        using var subChannel2 = await CreateSubscription($"q_rsp_{configuration.UserId}", configuration.ResponseExchange,
            configuration.UserId, connection, AmqpDestination.Request, cancellationToken);

        using var subChannel3 = await CreateSubscription($"q_sync_{configuration.UserId}", configuration.SyncExchange,
            configuration.UserId, connection, AmqpDestination.Request, cancellationToken);
        
        using var pubChannel = connection.CreateModel();
        pubChannel.ConfirmSelect();

        cancellationToken.ThrowIfCancellationRequested();

        await AmqpMessageLoop(connection, pubChannel, configuration, cancellationToken);
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea, IModel subChannel, AmqpDestination destination)
    {
        var message = new AmqpMessage(destination, ea.RoutingKey, ea.BasicProperties.Type, ea.BasicProperties.UserId)
        {
            Body = ea.Body.ToArray(),
        };
        try
        {
            bool success = await CallProcessHandler(message);
            if (success)
                subChannel.BasicAck(ea.DeliveryTag, false);
            else
                subChannel.BasicNack(ea.DeliveryTag, false, true);
        }
        catch (Exception e)
        {
            subChannel.BasicNack(ea.DeliveryTag, false, true);
            Trace.WriteLine($"Message processing failed: {e}");
        }

        await Task.Yield();
    }

    private async Task<bool> CallProcessHandler(AmqpMessage message)
    {
        Debug.WriteLine($"Incoming amqp message: {message.Text}");
        try
        {
            switch (message.Destination)
            {
                case AmqpDestination.Request:
                    if (callbackRequests != null)
                        return await callbackRequests(message);
                    return false;
                case AmqpDestination.Response:
                    if (callbackResponses != null)
                        return await callbackResponses(message);
                    return false;
                case AmqpDestination.Sync:
                    if (callbackSync != null)
                        return await callbackSync(message);
                    return false;
                default:
                    return false;
            }
        }
        catch (Exception e)
        {
            Trace.Write($"Error while processing {e}");
            return false;
        }
    }


    private Func<AmqpMessage, Task<bool>> callbackRequests;
    private Func<AmqpMessage, Task<bool>> callbackResponses;
    private Func<AmqpMessage, Task<bool>> callbackSync;

    public void RegisterMessageProcessorCallback(AmqpDestination destination, Func<AmqpMessage, Task<bool>> callback)
    {
        switch (destination)
        {
            case AmqpDestination.Request:
                callbackRequests = callback;
                break;
            case AmqpDestination.Response:
                callbackResponses = callback;
                break;
            case AmqpDestination.Sync:
                callbackSync = callback;
                break;
        }
    }
}