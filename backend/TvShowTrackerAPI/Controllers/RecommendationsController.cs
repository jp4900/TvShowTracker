using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TvShowTrackerAPI.Controllers;

/// <summary>
/// Recommendations controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly ILogger<RecommendationsController> _logger;

    public RecommendationsController(
        IRecommendationService recommendationService,
        ILogger<RecommendationsController> logger)
    {
        _recommendationService = recommendationService;
        _logger = logger;
    }

    /// <summary>
    /// Get personalized TV show recommendations
    /// </summary>
    /// <param name="count">Number of recommendations (default: 10, max: 50)</param>
    /// <returns>List of recommended TV shows with scores and reasons</returns>
    /// <response code="200">Returns recommendations</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TvShowRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TvShowRecommendationDto>>> GetRecommendations(
        [FromQuery] int count = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            count = Math.Min(count, 50); // 50 max

            var recommendations = await _recommendationService.GetRecommendationsAsync(userId.Value, count);

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations");
            return BadRequest(new { message = "Failed to get recommendations" });
        }
    }

    /// <summary>
    /// Train the ML recommendation model
    /// Should be called periodically or when significant data changes
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">Model trained successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("train")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> TrainModel()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            _logger.LogInformation("ML model training triggered by user {UserId}", userId);
            var trained = await _recommendationService.TrainModelAsync();

            if (!trained)
                return BadRequest(new { message = "Not enough data to train ML model" });

            return Ok(new { message = "ML model trained successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training ML model");
            return BadRequest(new { message = "Failed to train model" });
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