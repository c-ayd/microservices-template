using AuthService.Application.Dtos.Messaging.Templates;

namespace AuthService.Application.Abstractions.Messaging.Templates
{
    public interface IEmailTemplates
    {
        EmailTemplateDto GetEmailVerificationTemplate(string token, int expirationTimeInHours);
        EmailTemplateDto GetPasswordResetTemplate(string token, int expirationTimeInHours);
    }
}
