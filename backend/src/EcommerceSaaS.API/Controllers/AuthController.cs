using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceSaaS.Application.DTOs;
using EcommerceSaaS.Infrastructure.Services;

namespace EcommerceSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var (success, message, user) = await _authService.RegisterAsync(
            request.Email, request.FirstName, request.LastName, request.Password);

        if (!success)
            return BadRequest(ApiResponseDto<object>.ErrorResponse(message));

        var userDto = new UserDto
        {
            Id = user!.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = (int)user.Role,
            IsActive = user.IsActive
        };

        return CreatedAtAction(nameof(Register), ApiResponseDto<UserDto>.SuccessResponse(userDto, "User registered successfully"));
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var (success, message, accessToken, refreshToken) = await _authService.LoginAsync(request.Email, request.Password);

        if (!success)
            return Unauthorized(ApiResponseDto<object>.ErrorResponse(message));

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userDto = new UserDto { Id = Guid.Parse(userIdClaim?.Value ?? Guid.Empty.ToString()) };

        var response = new AuthResponseDto
        {
            AccessToken = accessToken!,
            RefreshToken = refreshToken!,
            User = userDto
        };

        return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(response, "Login successful"));
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        var (success, accessToken, refreshToken) = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!success)
            return Unauthorized(ApiResponseDto<object>.ErrorResponse("Refresh token invalid or expired"));

        var response = new AuthResponseDto
        {
            AccessToken = accessToken!,
            RefreshToken = refreshToken!
        };

        return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully"));
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userDto = new UserDto
        {
            Id = Guid.Parse(userId),
            Email = email ?? string.Empty,
            FirstName = name?.Split(' ').FirstOrDefault() ?? string.Empty,
            LastName = name?.Split(' ').LastOrDefault() ?? string.Empty
        };

        return Ok(ApiResponseDto<UserDto>.SuccessResponse(userDto));
    }
}
