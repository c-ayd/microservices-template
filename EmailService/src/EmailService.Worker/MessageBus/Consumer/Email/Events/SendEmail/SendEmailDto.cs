namespace EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail
{
    public class SendEmailDto : RabbitMqDtoBase
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
    }
}
