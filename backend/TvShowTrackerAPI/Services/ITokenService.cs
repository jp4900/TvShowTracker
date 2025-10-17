namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token (short-lived, 1 hour)
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <param name="email">User's email</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(Guid userId, string email);

    /// <summary>
    /// Generate cryptographically secure refresh token (long-lived, 7 days)
    /// </summary>
    /// <returns>Random token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate JWT access token and extract user ID
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>User ID if valid, null if invalid</returns>
    Guid? ValidateAccessToken(string token);
}