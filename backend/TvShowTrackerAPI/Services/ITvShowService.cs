using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for TV Show service
/// </summary>
public interface ITvShowService
{
    /// <summary>
    /// Get all TV shows with pagination, filtering, and sorting
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="genreId">Filter by genre (optional)</param>
    /// <param name="type">Filter by type (optional)</param>
    /// <param name="sortBy">Sort field (title, rating, popularity, ReleaseDate)</param>
    /// <param name="sortDescending">Sort direction</param>
    /// <param name="userId">Current user ID (for favorites)</param>
    Task<PaginatedResponse<TvShowListDto>> GetTvShowsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        Guid? genreId = null,
        string sortBy = "popularity",
        bool sortDescending = true,
        Guid? userId = null);

    /// <summary>
    /// Get TV show by ID
    /// </summary>
    Task<TvShowDto?> GetTvShowByIdAsync(Guid id, Guid? userId = null);

    /// <summary>
    /// Get episodes for a TV show
    /// </summary>
    /// <param name="tvShowId">TV Show ID</param>
    /// <param name="seasonNumber">Filter by season (optional)</param>
    Task<List<EpisodeDto>> GetEpisodesAsync(Guid tvShowId, int? seasonNumber = null);

    /// <summary>
    /// Get featured actors from TV show
    /// </summary>
    Task<List<ActorDto>> GetTvShowActorsAsync(Guid tvShowId);

    /// <summary>
    /// Search TV shows by title
    /// </summary>
    Task<PaginatedResponse<TvShowListDto>> SearchTvShowsAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 20,
        Guid? userId = null);
}