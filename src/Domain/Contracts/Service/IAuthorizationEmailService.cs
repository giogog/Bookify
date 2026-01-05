using Domain.Exception;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Contracts;

public interface IAuthorizationEmailService
{
    
    Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
    Task<EmailResult> CheckMailConfirmationAsync(string Username);
}
