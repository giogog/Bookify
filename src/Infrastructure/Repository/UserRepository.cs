using Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager) => _userManager = userManager;

    public async Task<IdentityResult> AddToRoleAsync(User user, string roles) => await _userManager.AddToRoleAsync(user, roles);

    public async Task<IdentityResult> DeleteUserRoleAsync(User user, string role) => await _userManager.RemoveFromRoleAsync(user, role);

    public async Task<bool> CheckPasswordAsync(User user, string password) => await _userManager.CheckPasswordAsync(user, password);

    public async Task<IdentityResult> CreateUserAsync(User user, string passord) => await _userManager.CreateAsync(user, passord);

    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userManager.Users.ToListAsync();

    public async Task<IEnumerable<User>> GetUsersWithConditionAsync(Expression<Func<User, bool>> expression) => await _userManager.Users.Where(expression).ToListAsync();

    public async Task<User?> GetUserAsync(Expression<Func<User, bool>> expression) => await _userManager.Users.FirstOrDefaultAsync(expression);

    public async Task<User?> GetUserFromClaimAsync(ClaimsPrincipal claimsPrincipal) => await _userManager.GetUserAsync(claimsPrincipal);

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) => await _userManager.ConfirmEmailAsync(user, token);

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword) => await _userManager.ResetPasswordAsync(user, token, newPassword);

    public async Task UpdateUserAsync(User user) => await _userManager.UpdateAsync(user);

    public IQueryable<User> Users() => _userManager.Users;


}
