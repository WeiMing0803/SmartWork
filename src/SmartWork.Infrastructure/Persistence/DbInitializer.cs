using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartWork.Infrastructure.Identity;

namespace SmartWork.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(RoleManager<ApplicationRole> roleManager, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        string[] roles = ["Admin", "Manager", "Member", "Guest"];
        foreach (string role in roles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;
            IdentityResult result = await roleManager.CreateAsync(new ApplicationRole { Name = role });
            if (result.Succeeded) logger?.LogInformation("已创建角色 {Role}", role);
            else logger?.LogWarning("创建角色 {Role} 失败：{Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
