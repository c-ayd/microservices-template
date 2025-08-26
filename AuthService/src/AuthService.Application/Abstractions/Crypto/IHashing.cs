using AuthService.Application.Dtos.Crypto.Enums;

namespace AuthService.Application.Abstractions.Crypto
{
    public interface IHashing
    {
        string HashPassword(string password);
        EPasswordVerificationResult VerifyPassword(string hashedPassword, string password);

        string HashSha256(string value);
        bool CompareSha256(string hashedValue, string value);
    }
}
