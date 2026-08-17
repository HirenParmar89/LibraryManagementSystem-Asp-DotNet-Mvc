namespace LibraryManagementSystem.Domain.Common;

/// <summary>
/// Base class for all entities. Provides a globally unique identifier.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}