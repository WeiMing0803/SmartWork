using SmartWork.Domain.Common;
using SmartWork.Domain.Enums;

namespace SmartWork.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssigneeId { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public DateTimeOffset? DueAt { get; set; }
    public Workspace? Workspace { get; set; }
    public ICollection<TaskComment> Comments { get; set; } = [];
    public ICollection<FileAsset> Attachments { get; set; } = [];
}
