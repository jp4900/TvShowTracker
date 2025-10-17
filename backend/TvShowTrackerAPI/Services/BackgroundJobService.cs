using Hangfire;
using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Background job service using Hangfire
/// 
/// Jobs:
/// 1. Send recommendation emails daily
/// 2. Train ML model weekly
/// 3. Prune expired tokens hourly
/// 4. Clean up old recommendations
/// </summary>
public class BackgroundJobService
{
    /// <summary>
    /// Called at application startup
    /// </summary>
    public static void RegisterJobs()
    {
        // Send recommendation emails daily at 9 AM
        RecurringJob.AddOrUpdate<BackgroundJobService>(
            "send-recommendation-emails",
            service => service.SendRecommendationEmailsAsync(),
            Cron.Daily(9),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // Train ML model weekly on Sundays at 2 AM
        RecurringJob.AddOrUpdate<BackgroundJobService>(
            "train-ml-model",
            service => service.TrainMLModelAsync(),
            Cron.Weekly(DayOfWeek.Sunday, 2),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // Prune expired tokens every hour
        RecurringJob.AddOrUpdate<BackgroundJobService>(
            "prune-expired-tokens",
            service => service.PruneExpiredTokensAsync(),
            Cron.Hourly,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // Generate recommendations for all users daily at 8 AM
        RecurringJob.AddOrUpdate<BackgroundJobService>(
            "generate-recommendations",
            service => service.GenerateRecommendationsAsync(),
            Cron.Daily(8),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(IServiceProvider serviceProvider, ILogger<BackgroundJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Send recommendation emails to all users
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task SendRecommendationEmailsAsync()
    {
        _logger.LogInformation("Starting recommendation email job...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        try
        {
            var usersWithRecommendations = await context.UserRecommendations
                .Include(r => r.User)
                .Include(r => r.TvShow)
                    .ThenInclude(t => t.Genres)
                        .ThenInclude(tg => tg.Genre)
                .Where(r => !r.EmailSent && !r.User.IsDeleted)
                .GroupBy(r => r.UserId)
                .ToListAsync();

            foreach (var userGroup in usersWithRecommendations)
            {
                var user = userGroup.First().User;
                var recommendations = userGroup.Select(r => new TvShowRecommendationDto
                {
                    Id = r.TvShow.Id,
                    Title = r.TvShow.Title,
                    PosterUrl = r.TvShow.PosterUrl,
                    Rating = r.TvShow.Rating,
                    ReleaseDate = r.TvShow.ReleaseDate,
                    Genres = r.TvShow.Genres.Select(g => g.Genre.Name).ToList(),
                    IsFavorite = false,
                    Score = r.Score,
                    Reason = r.Reason ?? "Recommended for you"
                }).ToList();

                // Send email
                await emailService.SendRecommendationEmailAsync(
                    user.Email,
                    recommendations
                );

                // Mark as sent
                foreach (var rec in userGroup)
                {
                    rec.EmailSent = true;
                    rec.EmailSentAt = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("Recommendation emails sent to {Count} users", usersWithRecommendations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending recommendation emails");
            throw; // Hangfire will retry
        }
    }

    /// <summary>
    /// Train ML model
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    public async Task TrainMLModelAsync()
    {
        _logger.LogInformation("Starting ML model training job...");

        using var scope = _serviceProvider.CreateScope();
        var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

        try
        {
            await recommendationService.TrainModelAsync();
            _logger.LogInformation("✅ ML model trained successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error training ML model");
            throw;
        }
    }

    /// <summary>
    /// Prune expired refresh tokens
    /// </summary>
    [AutomaticRetry(Attempts = 1)]
    public async Task PruneExpiredTokensAsync()
    {
        _logger.LogInformation("Starting token pruning job...");

        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        try
        {
            await authService.PruneExpiredTokensAsync();
            _logger.LogInformation("Expired tokens pruned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pruning tokens");
        }
    }

    /// <summary>
    /// Generate recommendations for all users
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    public async Task GenerateRecommendationsAsync()
    {
        _logger.LogInformation("Starting recommendation generation job...");

        using var scope = _serviceProvider.CreateScope();
        var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

        try
        {
            await recommendationService.GenerateRecommendationsForAllUsersAsync();
            _logger.LogInformation("Recommendations generated for all users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations");
            throw;
        }
    }
}