using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartWork.Application.Common.Interfaces;
using SmartWork.Domain.Common;
using SmartWork.Domain.Entities;
using SmartWork.Infrastructure.Identity;

namespace SmartWork.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentTag> DocumentTags => Set<DocumentTag>();
    public DbSet<DocumentHistory> DocumentHistories => Set<DocumentHistory>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<FileAsset> FileAssets => Set<FileAsset>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Workspace>(e => { e.Property(x => x.Name).HasMaxLength(128).IsRequired(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<WorkspaceMember>(e => { e.HasIndex(x => new { x.WorkspaceId, x.UserId }).IsUnique(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<Document>(e => { e.Property(x => x.Title).HasMaxLength(200).IsRequired(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<DocumentTag>(e => { e.Property(x => x.Name).HasMaxLength(64).IsRequired(); e.HasIndex(x => new { x.DocumentId, x.Name }).IsUnique(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<TaskItem>(e => { e.Property(x => x.Title).HasMaxLength(200).IsRequired(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<FileAsset>(e => { e.ToTable("Files"); e.Property(x => x.FileName).HasMaxLength(255).IsRequired(); e.Property(x => x.StoragePath).HasMaxLength(512).IsRequired(); e.HasQueryFilter(x => !x.IsDeleted); });
        builder.Entity<RefreshToken>(e => { e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired(); e.HasIndex(x => x.TokenHash).IsUnique(); });
        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) builder.Entity(entityType.ClrType).Property(nameof(BaseEntity.CreatedAt)).IsRequired();
        }
    }
}
