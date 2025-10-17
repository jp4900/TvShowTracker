using TvShowTrackerAPI.DTOs.Auth;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="dto">Registration data</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication response with new tokens</returns>
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    /// <param name="refreshToken">Token to revoke</param>
    Task RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Prune expired refresh tokens (runs as background job)
    /// </summary>
    Task PruneExpiredTokensAsync();
}