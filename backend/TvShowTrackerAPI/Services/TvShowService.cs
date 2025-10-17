using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;
using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;
using TvShowTrackerAPI.Models;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// TV Show service implementation
/// </summary>
public class TvShowService : ITvShowService
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<TvShowService> _logger;

    public TvShowService(
        AppDbContext context,
        ICacheService cache,
        ILogger<TvShowService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get TV shows with pagination, filtering, and sorting
    /// </summary>
    public async Task<PaginatedResponse<TvShowListDto>> GetTvShowsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        Guid? genreId = null,
        string sortBy = "popularity",
        bool sortDescending = true,
        Guid? userId = null)
    {
        var query = _context.TvShows
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .AsQueryable();

        // Filter by genre 
        if (genreId.HasValue)
        {
            query = query.Where(t => t.Genres.Any(g => g.GenreId == genreId.Value));
        }

        // Sorting
        query = sortBy.ToLower() switch
        {
            "title" => sortDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "rating" => sortDescending
                ? query.OrderByDescending(t => t.Rating)
                : query.OrderBy(t => t.Rating),
            "releasedate" => sortDescending
                ? query.OrderByDescending(t => t.ReleaseDate)
                        : query.OrderBy(t => t.ReleaseDate),
            "popularity" => sortDescending
                ? query.OrderByDescending(t => t.Popularity)
                : query.OrderBy(t => t.Popularity),
            _ => query.OrderByDescending(t => t.Popularity) // Default sort
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Pagination
        var tvShows = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get user's favorites if userId provided
        HashSet<Guid> userFavorites = new();
        if (userId.HasValue)
        {
            userFavorites = await _context.FavoriteTvShows
                .Where(f => f.UserId == userId.Value)
                .Select(f => f.TvShowId)
                .ToHashSetAsync();
        }

        // Map to DTOs
        var items = tvShows.Select(t => new TvShowListDto
        {
            Id = t.Id,
            Title = t.Title,
            PosterUrl = t.PosterUrl,
            Rating = t.Rating,
            ReleaseDate = t.ReleaseDate,
            Genres = t.Genres.Select(g => g.Genre.Name).ToList(),
            IsFavorite = userFavorites.Contains(t.Id)
        }).ToList();

        return PaginatedResponse<TvShowListDto>.Create(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Get TV show by ID with full details
    /// </summary>
    public async Task<TvShowDto?> GetTvShowByIdAsync(Guid id, Guid? userId = null)
    {
        // Try cache first
        var cacheKey = CacheKeys.TvShowById(id);
        var cached = await _cache.GetAsync<TvShowDto>(cacheKey);
        if (cached != null && !userId.HasValue)
            return cached;

        var tvShow = await _context.TvShows
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tvShow == null)
            return null;

        // Check if user has favorited this show
        bool isFavorite = false;
        if (userId.HasValue)
        {
            isFavorite = await _context.FavoriteTvShows
                .AnyAsync(f => f.UserId == userId.Value && f.TvShowId == id);
        }

        var dto = new TvShowDto
        {
            Id = tvShow.Id,
            Title = tvShow.Title,
            Description = tvShow.Description,
            ReleaseDate = tvShow.ReleaseDate,
            EndDate = tvShow.EndDate,
            Status = tvShow.Status.ToString(),
            Type = tvShow.Type.ToString(),
            PosterUrl = tvShow.PosterUrl,
            BackdropUrl = tvShow.BackdropUrl,
            Rating = tvShow.Rating,
            VoteCount = tvShow.VoteCount,
            NumberOfSeasons = tvShow.NumberOfSeasons,
            NumberOfEpisodes = tvShow.NumberOfEpisodes,
            Popularity = tvShow.Popularity,
            Genres = tvShow.Genres.Select(g => g.Genre.Name).ToList(),
            IsFavorite = isFavorite
        };

        // Cache if no user data
        if (!userId.HasValue)
        {
            await _cache.SetAsync(cacheKey, dto, CacheKeys.TvShowsCacheDuration);
        }

        return dto;
    }

    /// <summary>
    /// Get episodes for a TV show
    /// </summary>
    public async Task<List<EpisodeDto>> GetEpisodesAsync(Guid tvShowId, int? seasonNumber = null)
    {
        var query = _context.Episodes
            .Where(e => e.TvShowId == tvShowId);

        // Filter by season if specified
        if (seasonNumber.HasValue)
        {
            query = query.Where(e => e.SeasonNumber == seasonNumber.Value);
        }

        var episodes = await query
            .OrderBy(e => e.SeasonNumber)
            .ThenBy(e => e.EpisodeNumber)
            .ToListAsync();

        return episodes.Select(e => new EpisodeDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            SeasonNumber = e.SeasonNumber,
            EpisodeNumber = e.EpisodeNumber,
            ReleaseDate = e.ReleaseDate,
            Length = e.Length,
            StillPath = e.StillPath,
            Rating = e.Rating,
            VoteCount = e.VoteCount
        }).ToList();
    }

    /// <summary>
    /// Get featured actors from TV show
    /// </summary>
    public async Task<List<ActorDto>> GetTvShowActorsAsync(Guid tvShowId)
    {
        var actors = await _context.TvShowActors
            .Include(ta => ta.Actor)
            .Where(ta => ta.TvShowId == tvShowId)
            .OrderBy(ta => ta.Order) // Featured actors first
            .Take(20) // 20 actors limit
            .ToListAsync();

        return actors.Select(ta => new ActorDto
        {
            Id = ta.Actor.Id,
            Name = ta.Actor.Name,
            Biography = ta.Actor.Biography,
            DateOfBirth = ta.Actor.DateOfBirth,
            PlaceOfBirth = ta.Actor.PlaceOfBirth,
            ProfilePath = ta.Actor.ProfilePath,
            Popularity = ta.Actor.Popularity,
            CharacterName = ta.CharacterName
        }).ToList();
    }

    /// <summary>
    /// Search TV shows by title
    /// </summary>
    public async Task<PaginatedResponse<TvShowListDto>> SearchTvShowsAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 20,
        Guid? userId = null)
    {
        var searchQuery = _context.TvShows
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .Where(t => t.Title.ToLower().Contains(query.ToLower()))
            .OrderByDescending(t => t.Popularity);

        var totalCount = await searchQuery.CountAsync();

        var tvShows = await searchQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get user favorites
        HashSet<Guid> userFavorites = new();
        if (userId.HasValue)
        {
            userFavorites = await _context.FavoriteTvShows
                .Where(f => f.UserId == userId.Value)
                .Select(f => f.TvShowId)
                .ToHashSetAsync();
        }

        var items = tvShows.Select(t => new TvShowListDto
        {
            Id = t.Id,
            Title = t.Title,
            PosterUrl = t.PosterUrl,
            Rating = t.Rating,
            ReleaseDate = t.ReleaseDate,
            Genres = t.Genres.Select(g => g.Genre.Name).ToList(),
            IsFavorite = userFavorites.Contains(t.Id)
        }).ToList();

        return PaginatedResponse<TvShowListDto>.Create(items, totalCount, pageNumber, pageSize);
    }
}