using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartWork.Application.Auth;
using SmartWork.Domain.Entities;
using SmartWork.Infrastructure.Identity;
using SmartWork.Infrastructure.Persistence;
using SmartWork.Shared.Results;

namespace SmartWork.Infrastructure.Auth;

public sealed class AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = new() { UserName = request.UserName, Email = request.Email, DisplayName = request.UserName };
        IdentityResult result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return Result<AuthResponse>.Failure(result.Errors.Select(x => x.Description).ToArray());
        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password)) return Result<AuthResponse>.Failure("邮箱或密码不正确。");
        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    private async Task<Result<AuthResponse>> CreateAuthResponseAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        string accessToken = CreateAccessToken(user, expiresAt);
        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        dbContext.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = HashToken(refreshToken), ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays) });
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty, accessToken, refreshToken, expiresAt));
    }

    private string CreateAccessToken(ApplicationUser user, DateTimeOffset expiresAt)
    {
        Claim[] claims = [new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)];
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        JwtSecurityToken token = new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, expires: expiresAt.UtcDateTime, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
