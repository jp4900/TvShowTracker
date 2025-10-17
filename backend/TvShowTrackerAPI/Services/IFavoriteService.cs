using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for favorite TV shows management
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// Add TV show to user's favorites
    /// </summary>
    Task AddFavoriteAsync(Guid userId, Guid tvShowId);

    /// <summary>
    /// Remove TV show from user's favorites
    /// </summary>
    Task RemoveFavoriteAsync(Guid userId, Guid tvShowId);

    /// <summary>
    /// Get user's favorite TV shows
    /// </summary>
    Task<PaginatedResponse<TvShowListDto>> GetUserFavoritesAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Check if TV show is in user's favorites
    /// </summary>
    Task<bool> IsFavoriteAsync(Guid userId, Guid tvShowId);
}