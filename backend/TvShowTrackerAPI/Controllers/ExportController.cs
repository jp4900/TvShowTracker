using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TvShowTrackerAPI.Controllers;

/// <summary>
/// Export controller
/// Requirement #16: Export information to CSV/PDF
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IExportService exportService,
        IFavoriteService favoriteService,
        ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _favoriteService = favoriteService;
        _logger = logger;
    }

    /// <summary>
    /// Export user's complete data to CSV
    /// </summary>
    /// <returns>CSV file with user data</returns>
    /// <response code="200">Returns CSV file</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("my-data/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportMyDataToCsv()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var csvData = await _exportService.ExportUserDataToCsvAsync(userId.Value);

            _logger.LogInformation("User {UserId} exported data to CSV", userId);

            return File(csvData, "text/csv", $"my_data_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data to CSV");
            return BadRequest(new { message = "Failed to export data to CSV" });
        }
    }

    /// <summary>
    /// Export user's complete data to PDF
    /// </summary>
    /// <returns>PDF file with user data</returns>
    /// <response code="200">Returns PDF file</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("my-data/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportMyDataToPdf()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var pdfData = await _exportService.ExportUserDataToPdfAsync(userId.Value);

            _logger.LogInformation("User {UserId} exported data to PDF", userId);

            return File(pdfData, "application/pdf", $"my_data_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data to PDF");
            return BadRequest(new { message = "Failed to export data to PDF" });
        }
    }

    /// <summary>
    /// Export favorite TV shows to CSV
    /// </summary>
    /// <returns>CSV file with favorites</returns>
    /// <response code="200">Returns CSV file</response>
    [HttpGet("favorites/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportFavoritesToCsv()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            // Get user's favorite TV show IDs
            var favoriteIds = await GetUserFavoriteIdsAsync(userId.Value);

            var csvData = await _exportService.ExportTvShowsToCsvAsync(favoriteIds);

            return File(csvData, "text/csv", $"favorites_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting favorites to CSV");
            return BadRequest(new { message = "Failed to export favorites to CSV" });
        }
    }

    /// <summary>
    /// Export favorite TV shows to PDF
    /// </summary>
    /// <returns>PDF file with favorites</returns>
    /// <response code="200">Returns PDF file</response>
    [HttpGet("favorites/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportFavoritesToPdf()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var favoriteIds = await GetUserFavoriteIdsAsync(userId.Value);

            var pdfData = await _exportService.ExportTvShowsToPdfAsync(favoriteIds);

            return File(pdfData, "application/pdf", $"favorites_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting favorites to PDF");
            return BadRequest(new { message = "Failed to export favorites to PDF" });
        }
    }

    /// <summary>
    /// Helper: Get current user ID from JWT
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

    /// <summary>
    /// Helper: Get user's favorite TV show IDs
    /// </summary>
    private async Task<List<Guid>> GetUserFavoriteIdsAsync(Guid userId)
    {
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId, 1, 1000);
        return favorites.Items.Select(f => f.Id).ToList();
    }
}