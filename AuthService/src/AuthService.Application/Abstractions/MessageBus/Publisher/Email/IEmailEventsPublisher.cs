using AuthService.Application.Dtos.MessageBus.Publisher.Email;

namespace AuthService.Application.Abstractions.MessageBus.Publisher.Email
{
    public interface IEmailEventsPublisher
    {
        Task PublishSendEmailAsync(SendEmailDto dto, CancellationToken cancellationToken = default);
    }
}
