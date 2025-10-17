using TvShowTrackerAPI.DTOs.TvShows;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for recommendation service
/// </summary>
public interface IRecommendationService
{
	/// <summary>
	/// Generate personalized recommendations for a user
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="count">Number of recommendations (default: 10)</param>
	/// <returns>List of recommended TV shows with scores</returns>
	Task<List<TvShowRecommendationDto>> GetRecommendationsAsync(Guid userId, int count = 10);

	/// <summary>
	/// Generate and save recommendations for all users (background job)
	/// </summary>
	Task GenerateRecommendationsForAllUsersAsync();

	/// <summary>
	/// Train the recommendation model
	/// </summary>
	Task<bool> TrainModelAsync();
}

/// <summary>
/// DTO for TV show recommendation
/// </summary>
public class TvShowRecommendationDto : TvShowListDto
{
	/// <summary>
	/// Recommendation score (0-100)
	/// Higher = better match
	/// </summary>
	public decimal Score { get; set; }

	/// <summary>
	/// Human-readable reason for recommendation
	/// </summary>
	public string Reason { get; set; } = string.Empty;
}