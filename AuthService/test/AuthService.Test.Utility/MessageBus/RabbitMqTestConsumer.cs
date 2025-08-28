using AuthService.Infrastructure.MessageBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AuthService.Test.Utility.MessageBus
{
    public class RabbitMqTestConsumer : IAsyncDisposable
    {
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _queueName;

        private readonly TaskCompletionSource<BasicDeliverEventArgs> _tcs = new TaskCompletionSource<BasicDeliverEventArgs>();
        public async Task<BasicDeliverEventArgs> WaitForResult() => await _tcs.Task;

        public RabbitMqTestConsumer(RabbitMqConnection connection, string exchangeName, string exchangeType, string routingKey, string queueName)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;

            _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true).GetAwaiter().GetResult();
            _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
            _channel.QueueBindAsync(queueName, exchangeName, routingKey).GetAwaiter().GetResult();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                _tcs.SetResult(eventArgs);
            };

            _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();
        }
    }
}
