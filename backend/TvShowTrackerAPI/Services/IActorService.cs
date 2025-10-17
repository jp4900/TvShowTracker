using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for Actor service
/// </summary>
public interface IActorService
{
    /// <summary>
    /// Get all actors with pagination and search
    /// </summary>
    Task<PaginatedResponse<ActorListDto>> GetAllActorsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        string sortBy = "popularity",
        bool sortDescending = true);

    /// <summary>
    /// Get actor by ID with their TV shows
    /// </summary>
    Task<ActorDto?> GetActorByIdAsync(Guid id);

    /// <summary>
    /// Get TV shows an actor appeared in
    /// </summary>
    Task<List<ActorTvShowDto>> GetActorTvShowsAsync(Guid actorId);
}