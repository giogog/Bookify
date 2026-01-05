using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Contracts;

public interface IRoleRepository
{
    Task<bool> UserRoleExists(string roleName);
    Task<IdentityResult> CreateUserRoleAsync(Role role);
    Task<Role?> FindUserRolebyNameAsync(string role);
    Task<Role?> FindUserRoleByIdAsync(string role);
    Task<string?> GetRoleNameAsync(Role role);
}
