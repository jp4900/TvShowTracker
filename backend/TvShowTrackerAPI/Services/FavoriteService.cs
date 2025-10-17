using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;
using TvShowTrackerAPI.Models;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Favorite TV shows service implementation
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(AppDbContext context, ILogger<FavoriteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Add TV show to favorites
    /// </summary>
    public async Task AddFavoriteAsync(Guid userId, Guid tvShowId)
    {
        // Check if TV show exists
        var tvShowExists = await _context.TvShows.AnyAsync(t => t.Id == tvShowId);
        if (!tvShowExists)
            throw new InvalidOperationException("TV show not found");

        // Check if already favorited
        var exists = await _context.FavoriteTvShows
            .AnyAsync(f => f.UserId == userId && f.TvShowId == tvShowId);

        if (exists)
            return; // Already favorited

        // Add to favorites
        var favorite = new FavoriteTvShow
        {
            UserId = userId,
            TvShowId = tvShowId,
            AddedAt = DateTime.UtcNow
        };

        _context.FavoriteTvShows.Add(favorite);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} added TV show {TvShowId} to favorites", userId, tvShowId);
    }

    /// <summary>
    /// Remove TV show from favorites
    /// </summary>
    public async Task RemoveFavoriteAsync(Guid userId, Guid tvShowId)
    {
        var favorite = await _context.FavoriteTvShows
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TvShowId == tvShowId);

        if (favorite == null)
            return; // Not in favorites

        _context.FavoriteTvShows.Remove(favorite);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} removed TV show {TvShowId} from favorites", userId, tvShowId);
    }

    /// <summary>
    /// Get user's favorite TV shows with pagination
    /// </summary>
    public async Task<PaginatedResponse<TvShowListDto>> GetUserFavoritesAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var query = _context.FavoriteTvShows
            .Include(f => f.TvShow)
                .ThenInclude(t => t.Genres)
                    .ThenInclude(tg => tg.Genre)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt);

        var totalCount = await query.CountAsync();

        var favorites = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(f => f.TvShow)
            .ToListAsync();

        var items = favorites.Select(t => new TvShowListDto
        {
            Id = t.Id,
            Title = t.Title,
            PosterUrl = t.PosterUrl,
            Rating = t.Rating,
            ReleaseDate = t.ReleaseDate,
            Genres = t.Genres.Select(g => g.Genre.Name).ToList(),
            IsFavorite = true // Force true, all shows are favorites
        }).ToList();

        return PaginatedResponse<TvShowListDto>.Create(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Check if TV show is favorited by user
    /// </summary>
    public async Task<bool> IsFavoriteAsync(Guid userId, Guid tvShowId)
    {
        return await _context.FavoriteTvShows
            .AnyAsync(f => f.UserId == userId && f.TvShowId == tvShowId);
    }
}