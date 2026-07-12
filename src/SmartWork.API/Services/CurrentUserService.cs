using System.Security.Claims;
using SmartWork.Application.Common.Interfaces;

namespace SmartWork.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            string? value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out Guid userId) ? userId : null;
        }
    }
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
    public IReadOnlyCollection<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? [];
}
