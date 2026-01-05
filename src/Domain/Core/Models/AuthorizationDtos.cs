using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public record RegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; init; } = string.Empty;
}

public record LoginDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = string.Empty;
}


public record LoginResponseDto(Guid Id,string Username, string Token);
public record ResetPasswordDto(string Email, string Token, string NewPassword);

public record LoginWithTokenResult(bool Succeeded, string? ErrorCode, string? ErrorMessage, LoginResponseDto? Data);

public record PasswordResetRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; init; } = string.Empty;
}

