namespace TvShowTrackerAPI.DTOs.Auth;

/// <summary>
/// DTO for authentication responses (login/register success)
/// 
/// Returns:
/// - AccessToken: Short-lived JWT (1 hour) for API requests
/// - RefreshToken: Long-lived token (7 days) to get new access tokens
/// - User info
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT Access Token (valid for 1 hour)
    /// Include in Authorization header: "Bearer {token}"
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh Token (valid for 7 days)
    /// Used to obtain new access token when it expires
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// When the access token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// User information DTO (no sensitive data)
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
}