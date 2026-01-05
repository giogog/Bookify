using Domain.Entities;

namespace Contracts;

public interface ITokenGenerator
{
    Task<string> GenerateToken(User user);
    Task<string> GenerateMailTokenCode(User user);
    Task<string> GeneratePasswordResetToken(User user);

}
