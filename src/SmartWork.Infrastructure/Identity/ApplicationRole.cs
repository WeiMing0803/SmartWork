using Microsoft.AspNetCore.Identity;

namespace SmartWork.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
