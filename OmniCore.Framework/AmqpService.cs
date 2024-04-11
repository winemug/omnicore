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
    
    private readonly AsyncProducerConsumerQueue<AmqpMessage> _publishQueue;
    private readonly AsyncProducerConsumerQueue<Task> _confirmQueue;

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
        _confirmQueue = new AsyncProducerConsumerQueue<Task>();
    }

    public async Task Start()
    {
        _cts = new CancellationTokenSource();
        _amqpTask = Task.Run(async () => await StartJoin(_cts.Token));
    }

    public async Task Stop()
    {
        try
        {
            _cts?.Cancel();
            _amqpTask?.GetAwaiter().GetResult();
        }
        catch (TaskCanceledException) { }
        catch (Exception e)
        {
            Debug.WriteLine($"Error while cancelling core task: {e}");
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
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
    private async Task StartJoin(CancellationToken cancellationToken)
    {
        while (_appConfiguration.ClientAuthorization == null)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }

        AmqpEndpointDefinition? endpointDefinition = new AmqpEndpointDefinition
        {
        };
        
        var confirmationsTask = ConfirmationsTask(cancellationToken);
        while(true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await ConnectAndPublishLoop(endpointDefinition, cancellationToken);
            }
            catch(TaskCanceledException)
            {
                await confirmationsTask;
                throw;
            }
            catch(Exception e)
            {
                Trace.WriteLine($"Error while connectandpublish: {e.Message}");
                await Task.Delay(15000);
            }
        }
    }

    private async Task ConfirmationsTask(CancellationToken cancellationToken)
    {
        while(await _confirmQueue.OutputAvailableAsync(cancellationToken))
        {
            var confirmTask = await _confirmQueue.DequeueAsync(cancellationToken);
            try
            {
                await confirmTask;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Error in user function handling publish confirmation: {e.Message}");
            }
            await Task.Yield();
        }
    }
         private bool CertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain,
        SslPolicyErrors sslpolicyerrors)
        {
        return true;
        }
        private async Task ConnectAndPublishLoop(AmqpEndpointDefinition endpointDefinition,
          CancellationToken cancellationToken)
       {
        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(endpointDefinition.Dsn),
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = false,            
        };
        connectionFactory.Ssl.CertificateValidationCallback = CertificateValidationCallback;
        Debug.WriteLine("connecting");
        using var connection = Policy<IConnection>
            .Handle<Exception>()
            .WaitAndRetryForever(
            sleepDurationProvider: retries =>
            {
                return TimeSpan.FromSeconds(Math.Min(retries * 3, 60));
            },
            onRetry: (ex, ts) =>
            {
                Trace.WriteLine($"Error {ex}, waiting {ts} to reconnect");
            }).Execute((_) => { return connectionFactory.CreateConnection(); }, cancellationToken);
        Debug.WriteLine("connected");

        using var pubChannel = connection.CreateModel();
        pubChannel.ConfirmSelect();
        cancellationToken.ThrowIfCancellationRequested();

        var queueRequests = $"q_req_{endpointDefinition.UserId}";
        var queueResponses = $"q_rsp_{endpointDefinition.UserId}";
        var queueSync = $"q_sync_{endpointDefinition.UserId}";
        using (var createChannel = connection.CreateModel())
        {
            createChannel.QueueDeclare(queueRequests, true, true, false);
            createChannel.ExchangeDeclare(endpointDefinition.RequestExchange, "direct", true, false);
            createChannel.QueueBind(queueRequests, endpointDefinition.RequestExchange, endpointDefinition.UserId);

            createChannel.QueueDeclare(queueSync, true, true, false);
            createChannel.ExchangeDeclare(endpointDefinition.SyncExchange, "direct", true, false);
            createChannel.QueueBind(queueSync, endpointDefinition.SyncExchange, endpointDefinition.UserId);

            createChannel.QueueDeclare(queueResponses, true, true, false);
            createChannel.ExchangeDeclare(endpointDefinition.ResponseExchange, "direct", true, false);
            createChannel.QueueBind(queueResponses, endpointDefinition.ResponseExchange, endpointDefinition.UserId);
        }

        using var requestSubChannel = connection.CreateModel();
        var requestsConsumer = new AsyncEventingBasicConsumer(requestSubChannel);
        requestsConsumer.Received += async (sender, ea) => await ProcessMessage(ea, requestSubChannel, AmqpDestination.Request);
        requestSubChannel.BasicConsume(queueRequests, false, requestsConsumer);

        using var responseSubChannel = connection.CreateModel();
        var responseConsumer = new AsyncEventingBasicConsumer(requestSubChannel);
        responseConsumer.Received += async (sender, ea) => await ProcessMessage(ea, responseSubChannel, AmqpDestination.Response);
        responseSubChannel.BasicConsume(queueResponses, false, responseConsumer);

        using var syncSubChannel = connection.CreateModel();
        var syncConsumer = new AsyncEventingBasicConsumer(syncSubChannel);
        syncConsumer.Received += async (sender, ea) => await ProcessMessage(ea, responseSubChannel, AmqpDestination.Sync);
        syncSubChannel.BasicConsume(queueSync, false, syncConsumer);


        cancellationToken.ThrowIfCancellationRequested();


        while (true)
        {
            var result = false;
            try
            {
                result = await Policy.TimeoutAsync(30)
                    .ExecuteAsync((t) => _publishQueue.OutputAvailableAsync(t), cancellationToken);
            }
            catch (TimeoutRejectedException ex) { }

            if (!connection.IsOpen)
                break;

            if (result)
            {
                var message = await _publishQueue.DequeueAsync(cancellationToken);
                // cancellation ignored below this point
                try
                {
                    var properties = pubChannel.CreateBasicProperties();
                    properties.UserId = endpointDefinition.UserId;
                    properties.Type = message.Type;


                    var sequenceNo = pubChannel.NextPublishSeqNo;
                    Debug.WriteLine($"publishing seq {sequenceNo} {message.Text}");

                    pubChannel.BasicPublish(message.Exchange, message.Route, false,
                        properties, Encoding.UTF8.GetBytes(message.Text));
                    pubChannel.WaitForConfirmsOrDie();
                    if (message.OnPublishConfirmed != null)
                    {
                        await _confirmQueue.EnqueueAsync(message.OnPublishConfirmed);
                    }
                }
                catch
                {
                    await _publishQueue.EnqueueAsync(message);
                    throw;
                }
            }
            await Task.Yield();
        }
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea, IModel subChannel, AmqpDestination destination)
    {
        var message = new AmqpMessage(destination, ea.RoutingKey, ea.BasicProperties.Type, ea.BasicProperties.UserId)
        {
            Body = ea.Body.ToArray(),
        };
        try
        {
            bool success = await ProcessMessage(message);
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

    private async Task<bool> ProcessMessage(AmqpMessage message)
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
                callbackRequests = callback; break;
            case AmqpDestination.Response:
                callbackResponses = callback; break;
            case AmqpDestination.Sync:
                callbackSync = callback; break;
        }

    }
}