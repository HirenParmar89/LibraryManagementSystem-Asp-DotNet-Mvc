namespace LibraryManagementSystem.Domain.Common;

/// <summary>
/// Base class for entities that require tracking of creation and modification timestamps.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}