using AuthService.Application.Abstractions.MessageBus.Publisher.Email;
using AuthService.Application.Dtos.MessageBus.Publisher.Email;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;

namespace AuthService.Infrastructure.MessageBus.Publisher.Email
{
    public class EmailEventsPublisher : IEmailEventsPublisher, IAsyncDisposable
    {
        private readonly IChannel? _channel;

        public EmailEventsPublisher(RabbitMqConnection connection, ILoggerFactory loggerFactory)
        {
            try
            {
                _channel = connection.Connection!.CreateChannelAsync().GetAwaiter().GetResult();
                _channel.ExchangeDeclareAsync(EmailEventsConfigs.ExchangeName, ExchangeType.Topic, durable: true).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger(typeof(EmailEventsPublisher));
                logger.LogCritical(exception, "Could not create a channel: " + exception.Message);
            }
        }

        public async Task PublishSendEmailAsync(SendEmailDto dto, CancellationToken cancellationToken = default)
        {
#if DEBUG
            var useMessageBus = (bool?)AppDomain.CurrentDomain.GetData("UseMessageBus");
            if (!useMessageBus.HasValue || !useMessageBus.Value)
            {
                var isEmailSent = (bool?)AppDomain.CurrentDomain.GetData("SendEmailEventResult");
                if (isEmailSent.HasValue && !isEmailSent.Value)
                    throw new Exception("The email is not sent for testing.");

                return;
            }
#endif
            var body = JsonSerializer.SerializeToUtf8Bytes(dto);
            await _channel!.BasicPublishAsync(EmailEventsConfigs.ExchangeName, EmailEventsConfigs.SendEmailRoutingKey, body, cancellationToken);
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
