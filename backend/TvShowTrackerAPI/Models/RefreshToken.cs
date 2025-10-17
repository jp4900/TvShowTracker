namespace TvShowTrackerAPI.Models;

/// <summary>
/// Refresh Token entity for JWT authentication
/// 
/// HOW IT WORKS:
/// - User logs in → receives AccessToken (short-lived, 1 hour) + RefreshToken (long-lived, 7 days)
/// - AccessToken expires → user sends RefreshToken to get new AccessToken
/// - RefreshToken can be revoked for security
/// - Old tokens are pruned automatically
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Foreign key to User
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Refresh token string (cryptographically random)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// When this token expires (TTL - Time To Live)
    /// 7 days from creation
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this token has been revoked (invalidated for security)
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// When the token was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// If this token was replaced by a new one (token rotation)
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Check if token is currently active (not expired, not revoked)
    /// </summary>
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}