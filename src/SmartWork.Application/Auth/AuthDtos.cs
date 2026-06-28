namespace SmartWork.Application.Auth;

public sealed record RegisterRequest(string Email, string UserName, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);
public sealed record AuthResponse(Guid UserId, string Email, string UserName, string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
