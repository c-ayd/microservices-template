using AuthService.Application.Abstractions.MessageBus.Publisher.Email;
using AuthService.Application.Dtos.MessageBus.Publisher.Email;
using AuthService.Infrastructure.MessageBus;
using AuthService.Infrastructure.MessageBus.Publisher.Email;
using AuthService.Test.Integration.Infrastructure.Collections;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Fixtures.Hosting;
using AuthService.Test.Utility.MessageBus;
using Cayd.Test.Generators;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text.Json;

namespace AuthService.Test.Integration.Infrastructure.MessageBus.Publisher.Email
{
    [Collection(nameof(TestHostCollection))]
    public class EmailEventPublisherTest
    {
        private readonly TestHostFixture _testHostFixture;
        private readonly IEmailEventsPublisher _emailEventsPublisher;
        private readonly RabbitMqConnection _connection;

        public EmailEventPublisherTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _emailEventsPublisher = testHostFixture.Host.Services.GetRequiredService<IEmailEventsPublisher>();
            _connection = testHostFixture.Host.Services.GetRequiredService<RabbitMqConnection>();
        }

        [Fact]
        public async Task PublishSendEmailAsync_WhenMessageIsPublished_ShouldSendMessageToExchange()
        {
            // Arrange
            var consumer = new RabbitMqTestConsumer(_connection,
                EmailEventsConfigs.ExchangeName,
                ExchangeType.Topic,
                EmailEventsConfigs.SendEmailRoutingKey,
                queueName: "test.send-email.queue");

            var correlationId = Guid.NewGuid();
            var to = EmailGenerator.Generate();
            var subject = StringGenerator.GenerateUsingAlphanumeric(10);
            var body = StringGenerator.GenerateUsingAlphanumeric(10);

            EmailHelper.SetUseMessageBus(true);

            // Act
            await _emailEventsPublisher.PublishSendEmailAsync(new SendEmailDto()
            {
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow,
                To = to,
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            });

            var result = await consumer.WaitForResult();

            // Assert
            Assert.NotNull(result);

            var dto = JsonSerializer.Deserialize<SendEmailDto>(result.Body.Span);
            Assert.NotNull(dto);
            Assert.Equal(correlationId, dto.CorrelationId);
            Assert.Equal(to, dto.To);
            Assert.Equal(subject, dto.Subject);
            Assert.Equal(body, dto.Body);
            Assert.False(dto.IsBodyHtml, "The body is marked as HTML.");
        }
    }
}
