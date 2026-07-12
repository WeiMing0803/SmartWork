using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWork.Domain.Entities;
using SmartWork.Infrastructure.Identity;

namespace SmartWork.Infrastructure.Persistence.Configurations;

public sealed class DocumentHistoryConfiguration : IEntityTypeConfiguration<DocumentHistory>
{
    public void Configure(EntityTypeBuilder<DocumentHistory> builder)
    {
        // 历史表追加 only，但保留与 Document 一致的查询过滤器，避免 required 关系过滤警告。
        builder.HasQueryFilter(x => !x.IsDeleted);

        // 修改人外键：可空，SetNull 保留历史记录，与 AuditLog 一致
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
