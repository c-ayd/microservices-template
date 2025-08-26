using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;
using AuthService.Domain.Entities.UserManagement.Enums;

namespace AuthService.Application.Features.Commands.Authentication.SendEmail
{
    public class SendEmailRequest : IAsyncRequest<ExecResult<SendEmailResponse>>
    {
        public string? Email { get; set; }
        public ETokenPurpose? Purpose { get; set; }
    }
}
