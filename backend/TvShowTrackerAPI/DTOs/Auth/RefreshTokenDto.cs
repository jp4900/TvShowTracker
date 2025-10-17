using System.ComponentModel.DataAnnotations;

namespace TvShowTrackerAPI.DTOs.Auth;

/// <summary>
/// DTO for refreshing access token
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// The refresh token received during login/register
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}