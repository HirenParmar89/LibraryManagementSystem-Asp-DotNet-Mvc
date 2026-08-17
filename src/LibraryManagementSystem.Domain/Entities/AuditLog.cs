using LibraryManagementSystem.Domain.Common;

namespace LibraryManagementSystem.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string? UserId { get; set; } // Linked to IdentityUser
    public string Action { get; set; } = string.Empty;
    
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    
    public string? OldValues { get; set; } // JSON serialized
    public string? NewValues { get; set; } // JSON serialized
    
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}