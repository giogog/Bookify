using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;

namespace Contracts;

public interface IAuthorizationService
{

    Task<IdentityResult> Login(LoginDto loginDto);
    Task<LoginResponseDto> Authenticate(Expression<Func<User, bool>> expression);
    Task<IdentityResult> Register(RegisterDto registerDto);
    Task<IdentityResult> AddNewRole(string role);
    Task<IdentityResult> ResetPassword(ResetPasswordDto resetPasswordDto);

}
