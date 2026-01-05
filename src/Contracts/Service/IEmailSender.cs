using Domain.Exception;
namespace Contracts;

public interface IEmailSender
{
    Task<EmailResult> SendEmailAsync(string email, string subject, string htmlMessage);
}
