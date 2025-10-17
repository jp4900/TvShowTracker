namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for caching service
/// 
/// Used to cache:
/// - Genre list (rarely changes)
/// - Popular TV shows
/// - Actor information
/// - User session data
/// 
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get cached value by key
    /// </summary>
    /// <typeparam name="T">Type of cached object</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or null if not found/expired</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Set value in cache with optional TTL
    /// </summary>
    /// <typeparam name="T">Type of object to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expirationMinutes">TTL in minutes (default: 60)</param>
    Task SetAsync<T>(string key, T value, int expirationMinutes = 60) where T : class;

    /// <summary>
    /// Remove value from cache
    /// </summary>
    /// <param name="key">Cache key to remove</param>
    Task RemoveAsync(string key);

}