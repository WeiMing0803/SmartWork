using SmartWork.Domain.Common;

namespace SmartWork.Domain.Entities;

public class FileAsset : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public Guid? TaskItemId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public Workspace? Workspace { get; set; }
    public TaskItem? TaskItem { get; set; }
}
