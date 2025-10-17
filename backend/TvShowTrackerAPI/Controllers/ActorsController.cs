using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.DTOs.Actors;
using TvShowTrackerAPI.DTOs.Common;
using TvShowTrackerAPI.Services;

namespace TvShowTrackerAPI.Controllers;

/// <summary>
/// Actors controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ActorsController : ControllerBase
{
    private readonly IActorService _actorService;
    private readonly ILogger<ActorsController> _logger;

    public ActorsController(IActorService actorService, ILogger<ActorsController> logger)
    {
        _actorService = actorService;
        _logger = logger;
    }

    /// <summary>
    /// Get all actors with pagination, search, and sorting
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="search">Search by actor name (optional)</param>
    /// <param name="sortBy">Sort by: name, popularity, dateofbirth (default: popularity)</param>
    /// <param name="sortDesc">Sort descending (default: true)</param>
    /// <returns>Paginated list of actors</returns>
    /// <response code="200">Returns actors list</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResponse<ActorListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<ActorListDto>>> GetActors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string sortBy = "popularity",
        [FromQuery] bool sortDesc = true)
    {
        try
        {
            // Limit page size
            pageSize = Math.Min(pageSize, 100);

            var result = await _actorService.GetAllActorsAsync(
                pageNumber,
                pageSize,
                search,
                sortBy,
                sortDesc);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting actors");
            return BadRequest(new { message = "Failed to retrieve actors" });
        }
    }

    /// <summary>
    /// Get actor by ID with their TV shows
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <returns>Actor details with TV shows</returns>
    /// <response code="200">Returns actor details</response>
    /// <response code="404">Actor not found</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ActorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActorDto>> GetActorById(Guid id)
    {
        try
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            if (actor == null)
                return NotFound(new { message = "Actor not found" });

            return Ok(actor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting actor {Id}", id);
            return BadRequest(new { message = "Failed to retrieve actor" });
        }
    }

    /// <summary>
    /// Get TV shows an actor appeared in
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <returns>List of TV shows</returns>
    /// <response code="200">Returns TV shows list</response>
    [HttpGet("{id}/tvshows")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ActorTvShowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActorTvShowDto>>> GetActorTvShows(Guid id)
    {
        try
        {
            var tvShows = await _actorService.GetActorTvShowsAsync(id);
            return Ok(tvShows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TV shows for actor {Id}", id);
            return BadRequest(new { message = "Failed to retrieve TV shows" });
        }
    }
}