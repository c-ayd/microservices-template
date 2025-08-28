using Cayd.AspNetCore.FlexLog.Enums;
using Cayd.AspNetCore.FlexLog.Logging;
using Cayd.AspNetCore.FlexLog.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace EmailService.Worker.MessageBus
{
    public abstract class RabbitMqEventBase<T> : IAsyncDisposable
        where T : RabbitMqDtoBase
    {
        private readonly IChannel? _channel;
        private readonly string _queueName;
        private readonly RabbitMqDlxBase _dlx;
        private readonly FlexLogChannel _flexLogChannel;

        private readonly string? _protocolName;
        private readonly string? _logCategory;

        public RabbitMqEventBase(RabbitMqConnection connection,
            string exchangeName,
            string exchangeType,
            string queueName,
            string routingKey,
            RabbitMqDlxBase dlx,
            FlexLogChannel flexLogChannel,
            ILoggerFactory loggerFactory)
        {
            _queueName = queueName;
            _dlx = dlx;
            _flexLogChannel = flexLogChannel;

            try
            {
                _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
                _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true).GetAwaiter().GetResult();
                _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
                _channel.QueueBindAsync(queueName, exchangeName, routingKey).GetAwaiter().GetResult();

                _protocolName = $"{connection.Connection.Protocol.ApiName} {connection.Connection.Protocol.MajorVersion}.{connection.Connection.Protocol.MinorVersion}";
                _logCategory = typeof(RabbitMqEventBase<T>).FullName ?? typeof(RabbitMqEventBase<T>).Name;
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger(typeof(RabbitMqEventBase<T>));
                logger.LogCritical(exception, "Could not create a queue: " + exception.Message);
            }
        }

        protected abstract Task HandleMessageAsync(T dto);

        public async Task StartAsync()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var logContext = new FlexLogContext();
                logContext.Protocol = _protocolName!;

                T? dto = null;
                try
                {
                    dto = JsonSerializer.Deserialize<T>(eventArgs.Body.Span);
                    if (dto == null)
                        throw new NullReferenceException("After the deserilization, the DTO is null.");
                }
                catch (Exception exception)
                {
                    logContext.LogEntries.Add(new FlexLogEntry()
                    {
                        LogLevel = ELogLevel.Error,
                        Category = _logCategory,
                        Exception = exception,
                        Message = $"The body could not be deserialized to {nameof(T)}"
                    });

                    await _dlx.AddPoisonedMessage(eventArgs);
                    return;
                }

                logContext.CorrelationId = dto.CorrelationId;
                logContext.Timestamp = dto.Timestamp;

                try
                {
                    await HandleMessageAsync(dto);
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                    await _dlx.AddPoisonedMessage(eventArgs, dto.CorrelationId);

                    logContext.LogEntries.Add(new FlexLogEntry()
                    {
                        LogLevel = ELogLevel.Error,
                        Category = _logCategory,
                        Exception = exception,
                        Message = exception.Message
                    });
                }
                finally
                {
                    _flexLogChannel.AddLogContextToChannel(logContext);
                }
            };

            await _channel!.BasicConsumeAsync(_queueName, autoAck: false, consumer);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }
        }
    }
}
