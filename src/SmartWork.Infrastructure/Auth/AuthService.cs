using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken)) return Result<AuthResponse>.Failure("无效的刷新令牌。");

        // 从过期的 AccessToken 中取出用户 id（仅解析，不验签/验期）
        Guid userId;
        try
        {
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(request.AccessToken);
            string? sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (sub is null || !Guid.TryParse(sub, out userId)) return Result<AuthResponse>.Failure("无效的刷新令牌。");
        }
        catch (Exception)
        {
            return Result<AuthResponse>.Failure("无效的刷新令牌。");
        }

        string hash = HashToken(request.RefreshToken);
        RefreshToken? stored = await dbContext.RefreshTokens.AsTracking().FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);
        if (stored is null || stored.UserId != userId || !stored.IsActive) return Result<AuthResponse>.Failure("无效的刷新令牌。");

        // 令牌旋转：撤销旧 token，签发新对
        stored.RevokedAt = DateTimeOffset.UtcNow;
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result<AuthResponse>.Failure("无效的刷新令牌。");
        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken)) return Result.Success();
        string hash = HashToken(refreshToken);
        RefreshToken? stored = await dbContext.RefreshTokens.AsTracking().FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);
        if (stored is not null && stored.IsActive) stored.RevokedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<UserProfile>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result<UserProfile>.Failure("用户不存在。");
        return Result<UserProfile>.Success(new UserProfile(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty, user.DisplayName, user.AvatarUrl));
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Failure("用户不存在。");
        IdentityResult result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded) return Result.Failure(result.Errors.Select(x => x.Description).ToArray());

        // 改密后撤销该用户所有有效 RefreshToken，强制重新登录
        List<RefreshToken> tokens = await dbContext.RefreshTokens.AsTracking().Where(x => x.UserId == userId && x.RevokedAt == null).ToListAsync(cancellationToken);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        foreach (RefreshToken token in tokens) token.RevokedAt = now;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result<AuthResponse>> CreateAuthResponseAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        string accessToken = await CreateAccessTokenAsync(user, expiresAt);
        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        dbContext.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = HashToken(refreshToken), ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays) });
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty, accessToken, refreshToken, expiresAt));
    }

    private async Task<string> CreateAccessTokenAsync(ApplicationUser user, DateTimeOffset expiresAt)
    {
        List<Claim> claims = [new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)];
        IList<string> roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        JwtSecurityToken token = new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, expires: expiresAt.UtcDateTime, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
