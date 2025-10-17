namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for email service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send recommendation email to user
    /// </summary>
    /// <param name="toEmail">Recipient email address</param>
    /// <param name="recommendations">List of recommended TV shows</param>
    Task SendRecommendationEmailAsync(string toEmail, List<TvShowRecommendationDto> recommendations);

    /// <summary>
    /// Send welcome email after registration
    /// </summary>
    Task SendWelcomeEmailAsync(string toEmail);

    /// <summary>
    /// Test email connection
    /// </summary>
    Task<bool> TestConnectionAsync();
}