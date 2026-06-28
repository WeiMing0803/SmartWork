using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWork.Application.Common.Interfaces;
using SmartWork.Domain.Entities;
using SmartWork.Domain.Enums;

namespace SmartWork.API.Controllers;

[ApiController]
[Authorize]
[Route("api/workspaces")]
public sealed class WorkspacesController(IApplicationDbContext dbContext, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyWorkspaces(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId) return Unauthorized();
        List<WorkspaceSummaryResponse> workspaces = await dbContext.WorkspaceMembers
            .Where(x => x.UserId == userId)
            .Select(x => new WorkspaceSummaryResponse(x.WorkspaceId, x.Workspace!.Name, x.Workspace.Description, x.Role))
            .ToListAsync(cancellationToken);
        return Ok(workspaces);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId) return Unauthorized();
        Workspace workspace = new() { Name = request.Name, Description = request.Description, CreatedBy = userId };
        workspace.Members.Add(new WorkspaceMember { UserId = userId, Role = WorkspaceRole.Admin, CreatedBy = userId });
        dbContext.Workspaces.Add(workspace);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetMyWorkspaces), new { id = workspace.Id }, workspace);
    }
}

public sealed record CreateWorkspaceRequest(string Name, string? Description);

public sealed record WorkspaceSummaryResponse(Guid WorkspaceId, string Name, string? Description, WorkspaceRole Role);
