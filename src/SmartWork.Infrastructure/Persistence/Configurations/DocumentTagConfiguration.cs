using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWork.Domain.Entities;

namespace SmartWork.Infrastructure.Persistence.Configurations;

public sealed class DocumentTagConfiguration : IEntityTypeConfiguration<DocumentTag>
{
    public void Configure(EntityTypeBuilder<DocumentTag> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => new { x.DocumentId, x.Name }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
