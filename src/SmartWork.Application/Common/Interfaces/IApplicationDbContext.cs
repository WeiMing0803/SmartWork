using Microsoft.EntityFrameworkCore;
using SmartWork.Domain.Entities;

namespace SmartWork.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkspaceMember> WorkspaceMembers { get; }
    DbSet<Document> Documents { get; }
    DbSet<DocumentTag> DocumentTags { get; }
    DbSet<DocumentHistory> DocumentHistories { get; }
    DbSet<TaskItem> TaskItems { get; }
    DbSet<TaskComment> TaskComments { get; }
    DbSet<FileAsset> FileAssets { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
