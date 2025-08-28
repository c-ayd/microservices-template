using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailService.Worker.MessageBus
{
    public abstract class RabbitMqDlxBase : IAsyncDisposable
    {
        private readonly IChannel? _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMqDlxBase(RabbitMqConnection connection,
            string exchangeName,
            string exchangeType,
            string routingKey,
            ILoggerFactory loggerFactory)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;

            try
            {
                _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
                _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger(typeof(RabbitMqDlxBase));
                logger.LogCritical(exception, "Could not create a channel for DLX: " + exception.Message);
            }
        }

        public async Task AddPoisonedMessage(BasicDeliverEventArgs eventArgs, Guid? correlationId = null)
        {
            var properties = new BasicProperties()
            {
                CorrelationId = correlationId?.ToString()
            };

            await _channel!.BasicPublishAsync(_exchangeName, _routingKey, mandatory: false, properties, eventArgs.Body);
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
