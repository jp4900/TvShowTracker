using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Redis cache implementation
/// 
/// Cache Strategy:
/// - Genres: 24 hours (rarely change)
/// - TV Shows list: 1 hour (updated frequently)
/// - Actor details: 6 hours (occasionally updated)
/// - User favorites: No cache (data is in real time always)
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get cached value
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            // Deserialize from JSON
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Set value in cache with TTL
    /// </summary>
    public async Task SetAsync<T>(string key, T value, int expirationMinutes = 60) where T : class
    {
        try
        {
            // Serialize to JSON
            var serializedData = JsonSerializer.Serialize(value);

            // Set cache with TTL
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
            };

            await _cache.SetStringAsync(key, serializedData, options);

            _logger.LogDebug("Cache set: {Key}, TTL: {Minutes} minutes", key, expirationMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    /// <summary>
    /// Remove from cache
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Cache removed: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

}

/// <summary>
/// Cache key constants for consistency
/// </summary>
public static class CacheKeys
{
    public const string AllGenres = "genres:all";
    public const int GenresCacheDuration = 1440; // 24 hours

    public static string TvShowById(Guid id) => $"tvshow:{id}";
    public static string TvShowsByGenre(Guid genreId, int page) => $"tvshows:genre:{genreId}:page:{page}";
    public const int TvShowsCacheDuration = 60; // 1 hour

    public static string ActorById(Guid id) => $"actor:{id}";
    public const int ActorsCacheDuration = 360; // 6 hours

    public const string PopularShows = "tvshows:popular";
    public const int PopularShowsCacheDuration = 30;
}