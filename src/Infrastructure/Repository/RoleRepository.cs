using Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Infrastructure.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;

    public RoleRepository(RoleManager<Role> roleManager) => _roleManager = roleManager;

    public async Task<bool> UserRoleExists(string roleName) => await _roleManager.RoleExistsAsync(roleName);

    public async Task<IdentityResult> CreateUserRoleAsync(Role role) => await _roleManager.CreateAsync(role);

    public async Task<Role?> FindUserRolebyNameAsync(string role) => await _roleManager.FindByNameAsync(role);

    public async Task<Role?> FindUserRoleByIdAsync(string role) => await _roleManager.FindByIdAsync(role);

    public async Task<string?> GetRoleNameAsync(Role role) => await _roleManager.GetRoleNameAsync(role);

}
