using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public record RegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; init; }
}

public record LoginDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; }
}


public record LoginResponseDto(Guid Id,string Username, string Token);
public record ResetPasswordDto(string Email, string Token, string NewPassword);

