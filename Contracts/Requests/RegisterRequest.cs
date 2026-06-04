using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.Contracts.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(72, MinimumLength = 8, ErrorMessage = "Invalid password")]
    public string Password { get; set; } = string.Empty;
}