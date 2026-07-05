using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWork.Domain.Entities;
using SmartWork.Infrastructure.Identity;

namespace SmartWork.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        // 任务负责人外键：可空，不级联删除
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
            
        // 任务评论：级联删除
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.TaskItem)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // 任务附件：由 FileAssetConfiguration 统一配置
        // builder.HasMany(x => x.Attachments) ...
    }
}
