using Cayd.AspNetCore.FlexLog.Services;
using EmailService.Worker.Abstractions.Messaging;
using RabbitMQ.Client;

namespace EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail
{
    public class SendEmailEvent : RabbitMqEventBase<SendEmailDto>
    {
        private readonly IEmailSender _emailSender;

        public SendEmailEvent(IEmailSender emailSender,
            RabbitMqConnection connection, EmailEventsDlx dlx, FlexLogChannel flexLogChannel, ILoggerFactory loggerFactory)
            : base(connection, EmailEventsConfigs.ExchangeName, ExchangeType.Topic, SendEmailConfigs.QueueName, SendEmailConfigs.RoutingKey, dlx, flexLogChannel, loggerFactory)
        {
            _emailSender = emailSender;
        }

        protected override async Task HandleMessageAsync(SendEmailDto dto)
        {
            await _emailSender.SendAsync(dto.To, dto.Subject, dto.Body, dto.IsBodyHtml);
        }
    }
}
