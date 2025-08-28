using Cayd.AspNetCore.FlexLog.Services;
using EmailService.Test.Integration.Worker.Collections;
using EmailService.Test.Utility.Fixtures.Hosting;
using EmailService.Test.Utility.MessageBus;
using EmailService.Worker.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace EmailService.Test.Integration.Worker.MessageBus
{
    [Collection(nameof(TestHostCollection))]
    public class RabbitMqBaseTest : IDisposable
    {
        private const string _exchangeName = "test.events";
        private const string _queueName = "test.queue";
        private const string _routingKey = "test.test";

        private const string _poisonedExchangeName = "test.dlx";
        private const string _poisonedQueueName = "test.poisoned.queue";
        private const string _poisonedRoutingKey = "test.poisoned";

        private readonly TestHostFixture _testHostFixture;

        private readonly TestPoisonedDlx _dlx;
        private readonly RabbitMqTestPublisher<TestPoisonedDto> _publisher;
        private readonly TestPoisonedEvent _event;
        private readonly TestPoisonedDlxConsumer _dlxConsumer;

        public RabbitMqBaseTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;

            var connection = _testHostFixture.Host.Services.GetRequiredService<RabbitMqConnection>();
            var loggerFactory = _testHostFixture.Host.Services.GetRequiredService<ILoggerFactory>();
            var flexLogChannel = _testHostFixture.Host.Services.GetRequiredService<FlexLogChannel>();

            _dlx = new TestPoisonedDlx(connection, loggerFactory);
            _publisher = new RabbitMqTestPublisher<TestPoisonedDto>(connection, _exchangeName, ExchangeType.Topic, _routingKey);
            _event = new TestPoisonedEvent(connection, _dlx, flexLogChannel, loggerFactory);
            _dlxConsumer = new TestPoisonedDlxConsumer(connection);

            _event.StartAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _dlx.DisposeAsync().GetAwaiter().GetResult();
            _publisher.DisposeAsync().GetAwaiter().GetResult();
            _event.DisposeAsync().GetAwaiter().GetResult();
            _dlxConsumer.DisposeAsync().GetAwaiter().GetResult();
        }

        public class TestPoisonedDlx : RabbitMqDlxBase
        {
            public TestPoisonedDlx(RabbitMqConnection connection, ILoggerFactory loggerFactory)
                : base(connection, _poisonedExchangeName, ExchangeType.Topic, _poisonedRoutingKey, loggerFactory)
            {
            }
        }

        public class TestPoisonedEvent : RabbitMqEventBase<TestPoisonedDto>
        {
            public TestPoisonedEvent(RabbitMqConnection connection, TestPoisonedDlx dlx, FlexLogChannel flexLogChannel, ILoggerFactory loggerFactory)
                : base(connection, _exchangeName, ExchangeType.Topic, _queueName, _routingKey, dlx, flexLogChannel, loggerFactory)
            {
            }

            protected override async Task HandleMessageAsync(TestPoisonedDto dto)
            {
                throw new Exception("Test exception");
            }
        }

        public class TestPoisonedDlxConsumer : IAsyncDisposable
        {
            private readonly IChannel _channel;

            private readonly TaskCompletionSource<BasicDeliverEventArgs> _tcs = new TaskCompletionSource<BasicDeliverEventArgs>();
            public async Task<BasicDeliverEventArgs> WaitForResult() => await _tcs.Task;

            public TestPoisonedDlxConsumer(RabbitMqConnection connection)
            {
                _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
                _channel.ExchangeDeclareAsync(_poisonedExchangeName, ExchangeType.Topic, durable: true);
                _channel.QueueDeclareAsync(_poisonedQueueName, durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
                _channel.QueueBindAsync(_poisonedQueueName, _poisonedExchangeName, _poisonedRoutingKey).GetAwaiter().GetResult();

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    _tcs.SetResult(eventArgs);
                };

                _channel.BasicConsumeAsync(_poisonedQueueName, autoAck: true, consumer).GetAwaiter().GetResult();
            }

            public async ValueTask DisposeAsync()
            {
                await _channel.DisposeAsync();
            }
        }

        public class TestPoisonedDto : RabbitMqDtoBase
        {
            public required string Message { get; set; }
        }

        [Fact]
        public async Task RabbitMqEventBaseAndRabbitMqDlxBase_WhenMessageIsPoisoned_ShouldRedirectMessageToDlx()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var message = "test message";

            // Act
            await _publisher.PublishAsync(new TestPoisonedDto()
            {
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow,
                Message = message
            });

            var result = await _dlxConsumer.WaitForResult();

            // Assert
            Assert.NotNull(result);

            var dto = JsonSerializer.Deserialize<TestPoisonedDto>(result.Body.Span);
            Assert.NotNull(dto);
            Assert.Equal(correlationId, dto.CorrelationId);
            Assert.Equal(message, dto.Message);
        }
    }
}
