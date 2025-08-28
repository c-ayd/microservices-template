using Cayd.AspNetCore.FlexLog.Services;
using Cayd.Test.Generators;
using EmailService.Test.Integration.Worker.Collections;
using EmailService.Test.Utility;
using EmailService.Test.Utility.Fixtures.Hosting;
using EmailService.Test.Utility.MessageBus;
using EmailService.Worker.Abstractions.Messaging;
using EmailService.Worker.MessageBus;
using EmailService.Worker.MessageBus.Consumer.Email;
using EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail;
using EmailService.Worker.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EmailService.Test.Integration.Worker.MessageBus.Consumer.Email.Events
{
    [Collection(nameof(TestHostCollection))]
    public class SendEmailEventTest
    {
        private readonly TestHostFixture _testHostFixture;

        private readonly RabbitMqTestPublisher<SendEmailDto> _publisher;
        private readonly SendEmailEventTestImp? _event;

        public SendEmailEventTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;

            var emailSender = _testHostFixture.Host.Services.GetRequiredService<IEmailSender>();
            var connection = _testHostFixture.Host.Services.GetRequiredService<RabbitMqConnection>();
            var dlx = _testHostFixture.Host.Services.GetRequiredService<EmailEventsDlx>();
            var flexLogChannel = _testHostFixture.Host.Services.GetRequiredService<FlexLogChannel>();
            var loggerFactory = _testHostFixture.Host.Services.GetRequiredService<ILoggerFactory>();

            _publisher = new RabbitMqTestPublisher<SendEmailDto>(connection, EmailEventsConfigs.ExchangeName, ExchangeType.Topic, SendEmailConfigs.RoutingKey);
            _event = new SendEmailEventTestImp(emailSender, connection, dlx, flexLogChannel, loggerFactory);

            _event.StartAsync().GetAwaiter().GetResult();
        }

        public class SendEmailEventTestConnection : RabbitMqConnection
        {
            public SendEmailEventTestConnection(string connectionString, ILoggerFactory loggerFactory)
                : base(connectionString, loggerFactory)
            {
            }
        }

        public class SendEmailEventTestImp : SendEmailEvent
        {
            private readonly TaskCompletionSource _tcs = new TaskCompletionSource();
            public async Task WaitForResult() => await _tcs.Task;

            public SendEmailEventTestImp(IEmailSender emailSender, RabbitMqConnection connection, EmailEventsDlx dlx, FlexLogChannel flexLogChannel, ILoggerFactory loggerFactory)
                : base(emailSender, connection, dlx, flexLogChannel, loggerFactory)
            {
            }

            protected override async Task HandleMessageAsync(SendEmailDto dto)
            {
                await base.HandleMessageAsync(dto);
                _tcs.SetResult();
            }
        }

        [Fact]
        public async Task SendEmail_WhenMessageIsValid_ShouldSendEmail()
        {
            // Arrange
            var to = EmailGenerator.Generate();
            var subject = StringGenerator.GenerateUsingAlphanumeric(10);
            var body = StringGenerator.GenerateUsingAlphanumeric(10);
            var isBodyHtml = false;

            EmailHelper.SetEmailSenderResult(true);

            // Act
            await _publisher.PublishAsync(new SendEmailDto()
            {
                CorrelationId = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                To = to,
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            });

            await _event!.WaitForResult();

            // Assert
            var emailContent = await EmailHelper.GetLatestTempEmailFileAsync();

            Assert.NotNull(emailContent);
            Assert.NotNull(emailContent.ReceiverEmail);
            Assert.Equal(to, emailContent.ReceiverEmail);
            Assert.NotNull(emailContent.Subject);
            Assert.Equal(subject, emailContent.Subject);
            Assert.NotNull(emailContent.Body);
            Assert.Equal(body.TrimEnd('\r', '\n'), emailContent.Body);
            Assert.Equal(isBodyHtml ? EmailHelper.EmailContent.EContentType.Html : EmailHelper.EmailContent.EContentType.Plain, emailContent.ContentType);
        }
    }
}
