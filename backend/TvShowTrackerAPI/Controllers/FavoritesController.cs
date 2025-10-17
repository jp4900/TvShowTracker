using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;
using TvShowTrackerAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TvShowTrackerAPI.Controllers;


/// <summary>
/// Favorites controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
    {
        _favoriteService = favoriteService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's favorite TV shows
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20)</param>
    /// <returns>Paginated list of favorite TV shows</returns>
    /// <response code="200">Returns favorites list</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<TvShowListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedResponse<TvShowListDto>>> GetFavorites(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            pageSize = Math.Min(pageSize, 100);

            var favorites = await _favoriteService.GetUserFavoritesAsync(userId.Value, pageNumber, pageSize);
            return Ok(favorites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorites");
            return BadRequest(new { message = "Failed to retrieve favorites" });
        }
    }

    /// <summary>
    /// Add TV show to favorites
    /// </summary>
    /// <param name="tvShowId">TV show ID to add</param>
    /// <returns>Success message</returns>
    /// <response code="200">TV show added to favorites</response>
    /// <response code="400">Invalid TV show ID</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{tvShowId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> AddFavorite(Guid tvShowId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            await _favoriteService.AddFavoriteAsync(userId.Value, tvShowId);

            _logger.LogInformation("User {UserId} added TV show {TvShowId} to favorites", userId, tvShowId);

            return Ok(new { message = "TV show added to favorites" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite");
            return BadRequest(new { message = "Failed to add favorite" });
        }
    }

    /// <summary>
    /// Remove TV show from favorites
    /// </summary>
    /// <param name="tvShowId">TV show ID to remove</param>
    /// <returns>Success message</returns>
    /// <response code="200">TV show removed from favorites</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{tvShowId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> RemoveFavorite(Guid tvShowId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            await _favoriteService.RemoveFavoriteAsync(userId.Value, tvShowId);

            _logger.LogInformation("User {UserId} removed TV show {TvShowId} from favorites", userId, tvShowId);

            return Ok(new { message = "TV show removed from favorites" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing favorite");
            return BadRequest(new { message = "Failed to remove favorite" });
        }
    }

    /// <summary>
    /// Check if TV show is in favorites
    /// </summary>
    /// <param name="tvShowId">TV show ID</param>
    /// <returns>True if favorited, false otherwise</returns>
    /// <response code="200">Returns favorite status</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{tvShowId}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> IsFavorite(Guid tvShowId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tvShowId);

            return Ok(new { isFavorite });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status");
            return BadRequest(new { message = "Failed to check favorite status" });
        }
    }

    /// <summary>
    /// Helper method to get current user ID from JWT token
    /// </summary>
    private Guid? GetCurrentUserId()
    {

        if (User == null || !User.Identity.IsAuthenticated)
            return null;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}