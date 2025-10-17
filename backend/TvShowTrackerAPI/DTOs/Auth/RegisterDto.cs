using System.ComponentModel.DataAnnotations;

namespace TvShowTrackerAPI.DTOs.Auth;

/// <summary>
/// DTO for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// User's email address (used for login)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password (must meet security requirements)
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$",
        ErrorMessage = "Password must contain: uppercase, lowercase")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User must explicitly consent to data processing
    /// </summary>
    [Required(ErrorMessage = "You must accept data processing consent")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must consent to data processing")]
    public bool DataProcessingConsent { get; set; }
}