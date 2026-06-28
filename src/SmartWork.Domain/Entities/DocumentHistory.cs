using SmartWork.Domain.Common;

namespace SmartWork.Domain.Entities;

public class DocumentHistory : BaseEntity
{
    public Guid DocumentId { get; set; }
    public int Version { get; set; }
    public string ContentMarkdown { get; set; } = string.Empty;
    public Guid ModifiedBy { get; set; }
    public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;
    public Document? Document { get; set; }
}
