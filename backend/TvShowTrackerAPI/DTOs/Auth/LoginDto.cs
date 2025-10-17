using System.ComponentModel.DataAnnotations;

namespace TvShowTrackerAPI.DTOs.Auth;

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}