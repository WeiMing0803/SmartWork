using SmartWork.Domain.Common;

namespace SmartWork.Domain.Entities;

public class DocumentTag : BaseEntity
{
    public Guid DocumentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Document? Document { get; set; }
}
