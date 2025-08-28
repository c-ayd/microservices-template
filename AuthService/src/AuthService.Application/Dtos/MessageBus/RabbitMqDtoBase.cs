namespace AuthService.Application.Dtos.MessageBus
{
    public abstract class RabbitMqDtoBase
    {
        public required Guid CorrelationId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
