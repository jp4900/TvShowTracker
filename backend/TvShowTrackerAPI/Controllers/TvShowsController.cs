using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.DTOs.TvShows;
using TvShowTrackerAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TvShowTrackerAPI.Controllers;

/// <summary>
/// TV Shows controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TvShowsController : ControllerBase
{
	private readonly ITvShowService _tvShowService;
	private readonly ILogger<TvShowsController> _logger;

	public TvShowsController(ITvShowService tvShowService, ILogger<TvShowsController> logger)
	{
		_tvShowService = tvShowService;
		_logger = logger;
	}

	/// <summary>
	/// Get all TV shows with pagination, filtering, and sorting
	/// </summary>
	/// <param name="pageNumber">Page number (default: 1)</param>
	/// <param name="pageSize">Items per page (default: 20, max: 100)</param>
	/// <param name="genreId">Filter by genre ID (optional)</param>
	/// <param name="type">Filter by type (optional)</param>
	/// <param name="sortBy">Sort by: title, rating, popularity, ReleaseDate (default: popularity)</param>
	/// <param name="sortDesc">Sort descending (default: true)</param>
	/// <returns>Paginated list of TV shows</returns>
	/// <response code="200">Returns TV shows list</response>
	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(PaginatedResponse<TvShowListDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<PaginatedResponse<TvShowListDto>>> GetTvShows(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 20,
		[FromQuery] Guid? genreId = null,
		[FromQuery] string sortBy = "popularity",
		[FromQuery] bool sortDesc = true)
	{
		try
		{
			// Limit page size to keep performance
			pageSize = Math.Min(pageSize, 100);

			// Get current user ID if authenticated
			var userId = GetCurrentUserId();

			var result = await _tvShowService.GetTvShowsAsync(
				pageNumber,
				pageSize,
				genreId,
				sortBy,
				sortDesc,
                userId.Value);

			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting TV shows");
			return BadRequest(new { message = "Failed to retrieve TV shows" });
		}
	}

	/// <summary>
	/// Get TV show by ID
	/// </summary>
	/// <param name="id">TV show ID</param>
	/// <returns>TV show details</returns>
	/// <response code="200">Returns TV show details</response>
	/// <response code="404">TV show not found</response>
	[HttpGet("{id}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(TvShowDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<TvShowDto>> GetTvShowById(Guid id)
	{
		try
		{
            var userId = GetCurrentUserId();

            var tvShow = await _tvShowService.GetTvShowByIdAsync(id, userId.Value);

			if (tvShow == null)
				return NotFound(new { message = "TV show not found" });

			return Ok(tvShow);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting TV show {Id}", id);
			return BadRequest(new { message = "Failed to retrieve TV show" });
		}
	}

	/// <summary>
	/// Get episodes for a TV show
	/// </summary>
	/// <param name="id">TV show ID</param>
	/// <param name="season">Filter by season number (optional)</param>
	/// <returns>List of episodes</returns>
	/// <response code="200">Returns episodes list</response>
	[HttpGet("{id}/episodes")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(List<EpisodeDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<List<EpisodeDto>>> GetEpisodes(
		Guid id,
		[FromQuery] int? season = null)
	{
		try
		{
			var episodes = await _tvShowService.GetEpisodesAsync(id, season);
			return Ok(episodes);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting episodes for TV show {Id}", id);
			return BadRequest(new { message = "Failed to retrieve episodes" });
		}
	}

	/// <summary>
	/// Get featured actors from a TV show
	/// </summary>
	/// <param name="id">TV show ID</param>
	/// <returns>List of actors</returns>
	/// <response code="200">Returns actors list</response>
	[HttpGet("{id}/actors")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(List<ActorDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<List<ActorDto>>> GetTvShowActors(Guid id)
	{
		try
		{
			var actors = await _tvShowService.GetTvShowActorsAsync(id);
			return Ok(actors);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting actors for TV show {Id}", id);
			return BadRequest(new { message = "Failed to retrieve actors" });
		}
	}

	/// <summary>
	/// Search TV shows by title
	/// </summary>
	/// <param name="query">Search query</param>
	/// <param name="pageNumber">Page number (default: 1)</param>
	/// <param name="pageSize">Items per page (default: 20)</param>
	/// <returns>Paginated search results</returns>
	/// <response code="200">Returns search results</response>
	[HttpGet("search")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(PaginatedResponse<TvShowListDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<PaginatedResponse<TvShowListDto>>> SearchTvShows(
		[FromQuery] string query,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 20)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(query))
				return BadRequest(new { message = "Search query is required" });

			pageSize = Math.Min(pageSize, 100);

            var userId = GetCurrentUserId();

            var result = await _tvShowService.SearchTvShowsAsync(query, pageNumber, pageSize, userId.Value);
			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error searching TV shows with query: {Query}", query);
			return BadRequest(new { message = "Search failed" });
		}
	}

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