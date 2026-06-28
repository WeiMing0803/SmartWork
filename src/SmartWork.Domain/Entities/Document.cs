using SmartWork.Domain.Common;

namespace SmartWork.Domain.Entities;

public class Document : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public Workspace? Workspace { get; set; }
    public ICollection<DocumentTag> Tags { get; set; } = [];
    public ICollection<DocumentHistory> Histories { get; set; } = [];
}
