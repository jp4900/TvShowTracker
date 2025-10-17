namespace TvShowTrackerAPI.Models;

/// <summary>
/// User entity - stores user information, authentication data, and GDPR compliance fields
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User's email address (unique, used for login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password using BCrypt
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Last time user logged in (for security tracking)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// User consent to data processing (required by GDPR)
    /// </summary>
    public bool DataProcessingConsent { get; set; }

    /// <summary>
    /// When the user gave consent
    /// </summary>
    public DateTime? ConsentDate { get; set; }

    /// <summary>
    /// Token for data export requests (GDPR right to data portability)
    /// </summary>
    public string? DataExportToken { get; set; }

    /// <summary>
    /// When user requested data export
    /// </summary>
    public DateTime? DataExportRequestedAt { get; set; }

    /// <summary>
    /// When user requested account deletion (GDPR right to be forgotten)
    /// </summary>
    public DateTime? AccountDeletionRequestedAt { get; set; }

    // ===========================================
    // NAVIGATION PROPERTIES (Relationships)
    // ===========================================

    /// <summary>
    /// Refresh tokens for this user (for JWT token refresh)
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// User's favorite TV shows
    /// </summary>
    public ICollection<FavoriteTvShow> FavoriteTvShows { get; set; } = new List<FavoriteTvShow>();

    /// <summary>
    /// TV show recommendations for this user
    /// </summary>
    public ICollection<UserRecommendation> Recommendations { get; set; } = new List<UserRecommendation>();
}