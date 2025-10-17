using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTrackerAPI.DTOs.Auth;
using TvShowTrackerAPI.Services;

namespace TvShowTrackerAPI.Controllers;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="dto">Registration details</param>
    /// <returns>Authentication tokens</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid input or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var response = await _authService.RegisterAsync(dto);

            _logger.LogInformation("User registered: {Email}", dto.Email);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return BadRequest(new { message = "Registration failed. Please try again." });
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication tokens</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var response = await _authService.LoginAsync(dto);

            _logger.LogInformation("User logged in: {Email}", dto.Email);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return BadRequest(new { message = "Login failed. Please try again." });
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="dto">Refresh token</param>
    /// <returns>New authentication tokens</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(dto.RefreshToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return BadRequest(new { message = "Token refresh failed. Please try again." });
        }
    }

    /// <summary>
    /// Logout user (revoke refresh token)
    /// </summary>
    /// <param name="dto">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="400">Invalid token</response>
    [HttpPost("logout")]
    [Authorize] // User must be authenticated
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        try
        {
            await _authService.RevokeTokenAsync(dto.RefreshToken);

            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return BadRequest(new { message = "Logout failed. Please try again." });
        }
    }

    /// <summary>
    /// Test endpoint to verify authentication
    /// </summary>
    /// <returns>User information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;

        return Ok(new
        {
            userId,
            email,
            message = "You are authenticated!"
        });
    }
}