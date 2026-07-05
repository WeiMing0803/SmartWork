using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWork.Domain.Entities;

namespace SmartWork.Infrastructure.Persistence.Configurations;

public sealed class DocumentHistoryConfiguration : IEntityTypeConfiguration<DocumentHistory>
{
    public void Configure(EntityTypeBuilder<DocumentHistory> builder)
    {
        // 与 Document 的查询过滤器保持一致，避免 Required 关系警告
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
