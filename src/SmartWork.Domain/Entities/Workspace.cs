using SmartWork.Domain.Common;

namespace SmartWork.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<WorkspaceMember> Members { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<FileAsset> Files { get; set; } = [];
}
