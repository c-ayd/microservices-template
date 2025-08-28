namespace AuthService.Application.Dtos.MessageBus.Publisher.Email
{
    public class SendEmailDto : RabbitMqDtoBase
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
    }
}
