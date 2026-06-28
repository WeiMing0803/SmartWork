using Microsoft.AspNetCore.Identity;
using SmartWork.Domain.Entities;

namespace SmartWork.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
