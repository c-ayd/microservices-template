using EmailService.Worker.MessageBus;
using RabbitMQ.Client;
using System.Text.Json;

namespace EmailService.Test.Utility.MessageBus
{
    public class RabbitMqTestPublisher<T> : IAsyncDisposable
        where T : RabbitMqDtoBase
    {
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMqTestPublisher(RabbitMqConnection connection, string exchangeName, string exchangeType, string routingKey)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;

            _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true).GetAwaiter().GetResult();
        }

        public async Task PublishAsync(T dto)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(dto);
            await _channel.BasicPublishAsync(_exchangeName, _routingKey, body);
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();
        }
    }
}
