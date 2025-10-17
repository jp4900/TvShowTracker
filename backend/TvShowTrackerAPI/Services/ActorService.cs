using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;
using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Actor service implementation
/// </summary>
public class ActorService : IActorService
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<ActorService> _logger;

    public ActorService(
        AppDbContext context,
        ICacheService cache,
        ILogger<ActorService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get all actors with pagination, search, and sorting
    /// </summary>
    public async Task<PaginatedResponse<ActorListDto>> GetAllActorsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        string sortBy = "popularity",
        bool sortDescending = true)
    {
        var query = _context.Actors
            .Include(a => a.TvShows)
            .AsQueryable();

        // Search by name
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => a.Name.ToLower().Contains(search.ToLower()));
        }

        // Sorting
        query = sortBy.ToLower() switch
        {
            "name" => sortDescending
                ? query.OrderByDescending(a => a.Name)
                : query.OrderBy(a => a.Name),
            "popularity" => sortDescending
                ? query.OrderByDescending(a => a.Popularity ?? 0)
                : query.OrderBy(a => a.Popularity ?? 0),
            "dateofbirth" => sortDescending
                ? query.OrderByDescending(a => a.DateOfBirth ?? DateTime.MinValue)
                : query.OrderBy(a => a.DateOfBirth ?? DateTime.MaxValue),
            _ => query.OrderByDescending(a => a.Popularity ?? 0) // Default sort by popularity
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Pagination
        var actors = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map to DTOs
        var items = actors.Select(a => new ActorListDto
        {
            Id = a.Id,
            Name = a.Name,
            ProfilePath = a.ProfilePath,
            Popularity = a.Popularity,
            DateOfBirth = a.DateOfBirth,
            PlaceOfBirth = a.PlaceOfBirth,
            TvShowCount = a.TvShows.Count
        }).ToList();

        return PaginatedResponse<ActorListDto>.Create(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Get actor by ID with full details
    /// </summary>
    public async Task<ActorDto?> GetActorByIdAsync(Guid id)
    {
        // Try cache first
        var cacheKey = CacheKeys.ActorById(id);
        var cached = await _cache.GetAsync<ActorDto>(cacheKey);
        if (cached != null)
            return cached;

        var actor = await _context.Actors
            .Include(a => a.TvShows)
                .ThenInclude(ta => ta.TvShow)
                    .ThenInclude(t => t.Genres)
                        .ThenInclude(tg => tg.Genre)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (actor == null)
            return null;

        var dto = new ActorDto
        {
            Id = actor.Id,
            Name = actor.Name,
            Biography = actor.Biography,
            DateOfBirth = actor.DateOfBirth,
            PlaceOfBirth = actor.PlaceOfBirth,
            ProfilePath = actor.ProfilePath,
            Popularity = actor.Popularity,
            TvShows = actor.TvShows.Select(ta => new ActorTvShowDto
            {
                Id = ta.TvShow.Id,
                Title = ta.TvShow.Title,
                PosterUrl = ta.TvShow.PosterUrl,
                CharacterName = ta.CharacterName,
                Rating = ta.TvShow.Rating
            })
            .OrderByDescending(t => t.Rating)
            .ToList()
        };

        // Cache actor data
        await _cache.SetAsync(cacheKey, dto, CacheKeys.ActorsCacheDuration);

        return dto;
    }

    /// <summary>
    /// Get TV shows an actor appeared in
    /// </summary>
    public async Task<List<ActorTvShowDto>> GetActorTvShowsAsync(Guid actorId)
    {
        var tvShows = await _context.TvShowActors
            .Include(ta => ta.TvShow)
            .Where(ta => ta.ActorId == actorId)
            .OrderByDescending(ta => ta.TvShow.Rating)
            .Select(ta => new ActorTvShowDto
            {
                Id = ta.TvShow.Id,
                Title = ta.TvShow.Title,
                PosterUrl = ta.TvShow.PosterUrl,
                CharacterName = ta.CharacterName,
                Rating = ta.TvShow.Rating
            })
            .ToListAsync();

        return tvShows;
    }
}