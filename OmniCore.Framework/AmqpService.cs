using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OmniCore.Services.Interfaces;
using OmniCore.Services.Interfaces.Amqp;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OmniCore.Services;

public class AmqpService : IAmqpService
{
    public string Dsn { get; set; }
    public string Exchange { get; set; }
    public string Queue { get; set; }
    public string UserId { get; set; }
    
    private Task _amqpTask;
    private CancellationTokenSource _cts;
    private readonly SortedList<DateTimeOffset, string> _processedMessages;
    private readonly AsyncProducerConsumerQueue<AmqpMessage> _publishQueue;

    private ConcurrentBag<Func<AmqpMessage, Task<bool>>> _messageProcessors;
    public AmqpService()
    {
        _publishQueue = new AsyncProducerConsumerQueue<AmqpMessage>();
        _processedMessages = new SortedList<DateTimeOffset, string>();
        _messageProcessors = new ConcurrentBag<Func<AmqpMessage, Task<bool>>>();
    }

    public void SetEndpoint(AmqpEndpoint endpoint)
    {
        Dsn = endpoint.Dsn;
        Exchange = endpoint.Exchange;
        Queue = endpoint.Queue;
        UserId = endpoint.UserId;
    }
    
    public async Task Start()
    {
        _cts = new CancellationTokenSource();
        _amqpTask = Task.Run(async () => await AmqpTask(_cts.Token));
    }

    public async Task Stop()
    {
        try
        {
            _cts?.Cancel();
            _amqpTask?.GetAwaiter().GetResult();
        }
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

    private async Task AmqpTask(CancellationToken cancellationToken)
    {
        var cf = new ConnectionFactory
        {
            Uri = new Uri(Dsn),
            DispatchConsumersAsync = true
        };

        IConnection connection = null;
        try
        {
            while (connection == null)
                try
                {
                    connection = cf.CreateConnection();
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Connection failed {e}");
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
        }
        catch (TaskCanceledException)
        {
            return;
        }

        var subChannel = connection.CreateModel();
        var consumer = new AsyncEventingBasicConsumer(subChannel);
        consumer.Received += async (sender, ea) =>
        {
            var message = new AmqpMessage
            {
                Body = ea.Body.ToArray(),
                Id = ea.BasicProperties.MessageId
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
        };
        subChannel.BasicConsume(Queue, false, consumer);

        var pendingConfirmations = new ConcurrentDictionary<ulong, AmqpMessage>();
        var pubChannel = connection.CreateModel();
        pubChannel.ConfirmSelect();

        while (true)
            try
            {
                var processQueue = false;
                if (pendingConfirmations.IsEmpty)
                    processQueue = await _publishQueue.OutputAvailableAsync(cancellationToken);
                else
                    using (var queueTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                    {
                        try
                        {
                            processQueue = await _publishQueue.OutputAvailableAsync(queueTimeout.Token);
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    }

                if (processQueue)
                {
                    var message = await _publishQueue.DequeueAsync(cancellationToken);
                    var sequenceNo = pubChannel.NextPublishSeqNo;

                    try
                    {
                        var properties = pubChannel.CreateBasicProperties();
                        properties.MessageId = message.Id;
                        properties.UserId = UserId;
                        properties.Headers = message.Headers;
                        pendingConfirmations.TryAdd(sequenceNo, message);
                        pubChannel.BasicPublish(Exchange, message.Route, false,
                            properties, message.Body);
                        Debug.WriteLine($"published message: {message.Text}");
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine($"Error while publishing: {e}");
                        pendingConfirmations.TryRemove(sequenceNo, out message);
                        await _publishQueue.EnqueueAsync(message, cancellationToken);
                    }
                }
                else
                {
                    if (!pendingConfirmations.IsEmpty)
                    {
                        try
                        {
                            Debug.WriteLine("Starting confirmations");
                            pubChannel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(15));
                            Debug.WriteLine("Confirmation succeeded");
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine($"Error while confirming: {e}");
                            foreach (var pendingMessage in pendingConfirmations.Values)
                                _publishQueue.Enqueue(pendingMessage);
                        }

                        foreach (var message in pendingConfirmations.Values)
                        {
                            if (message.OnPublishConfirmed != null)
                                message.OnPublishConfirmed(message);
                        }

                        pendingConfirmations.Clear();
                    }
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }

        subChannel.Close();
        pubChannel.Close();
        connection.Close();
        connection.Dispose();
    }

    private async Task<bool> ProcessMessage(AmqpMessage message)
    {
        if (!string.IsNullOrEmpty(message.Id) && _processedMessages.ContainsValue(message.Id))
        {
            Debug.WriteLine($"Message with id {message.Id} already processed");
            return true;
        }

        Debug.WriteLine($"Incoming amqp message: {message.Text}");
        var processed = false;
        foreach (var pf in _messageProcessors)
        {
            try
            {
                processed = await pf(message);
                if (processed)
                {
                    _processedMessages.Add(DateTimeOffset.Now, message.Id);
                    break;
                }
            }
            catch (Exception e)
            {
                Trace.Write($"Error while processing {e}");
            }
        }

        var keysToRemove = _processedMessages.Where(p =>
                p.Key < DateTimeOffset.Now - TimeSpan.FromMinutes(15))
            .Select(p => p.Key)
            .ToArray();
        foreach (var key in keysToRemove) _processedMessages.Remove(key);

        return processed;
    }
    
    public void RegisterMessageProcessor(Func<AmqpMessage, Task<bool>> function)
    {
        _messageProcessors.Add(function);
    }
}