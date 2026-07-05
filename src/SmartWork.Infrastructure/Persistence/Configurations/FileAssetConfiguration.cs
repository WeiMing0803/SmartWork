using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWork.Domain.Entities;

namespace SmartWork.Infrastructure.Persistence.Configurations;

public sealed class FileAssetConfiguration : IEntityTypeConfiguration<FileAsset>
{
    public void Configure(EntityTypeBuilder<FileAsset> builder)
    {
        builder.ToTable("Files");
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(512).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        // 关联任务：可空，Restrict 避免多条级联路径冲突
        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
