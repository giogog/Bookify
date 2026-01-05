using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Contracts;

public interface IUserRepository
{
    Task<IdentityResult> CreateUserAsync(User user, string passord);
    Task<User?> GetUserAsync(Expression<Func<User, bool>> expression);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersWithConditionAsync(Expression<Func<User, bool>> expression);
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<User?> GetUserFromClaimAsync(ClaimsPrincipal claimsPrincipal);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
    Task<IdentityResult> DeleteUserRoleAsync(User user, string role);
    Task UpdateUserAsync(User user);
    IQueryable<User> Users();
}
