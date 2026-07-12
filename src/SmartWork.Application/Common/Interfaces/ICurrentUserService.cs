using System.Security.Claims;
using SmartWork.Application.Common.Interfaces;

namespace SmartWork.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Roles { get; }
}
