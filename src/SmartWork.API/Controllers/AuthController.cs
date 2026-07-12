using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWork.API.Extensions;
using SmartWork.Application.Auth;
using SmartWork.Application.Common.Interfaces;
using SmartWork.Shared.Results;

namespace SmartWork.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, IValidator<RegisterRequest> validator, CancellationToken cancellationToken)
    {
        ValidationResult validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return BadRequest(new { errors = validation.ToDictionary() });
        Result<AuthResponse> result = await authService.RegisterAsync(request, cancellationToken);
        return result.ToActionResult(value => Ok(value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, IValidator<LoginRequest> validator, CancellationToken cancellationToken)
    {
        ValidationResult validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return BadRequest(new { errors = validation.ToDictionary() });
        Result<AuthResponse> result = await authService.LoginAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(new { errors = result.Errors });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, IValidator<RefreshTokenRequest> validator, CancellationToken cancellationToken)
    {
        ValidationResult validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return BadRequest(new { errors = validation.ToDictionary() });
        Result<AuthResponse> result = await authService.RefreshAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(new { errors = result.Errors });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(request.RefreshToken, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (User.Identity?.Name is null) return Unauthorized();
        string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (!Guid.TryParse(userId, out Guid id)) return Unauthorized();
        Result<UserProfile> result = await authService.GetProfileAsync(id, cancellationToken);
        return result.ToActionResult(value => Ok(value));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, IValidator<ChangePasswordRequest> validator, CancellationToken cancellationToken)
    {
        ValidationResult validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return BadRequest(new { errors = validation.ToDictionary() });
        string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (!Guid.TryParse(userId, out Guid id)) return Unauthorized();
        Result result = await authService.ChangePasswordAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }
}
