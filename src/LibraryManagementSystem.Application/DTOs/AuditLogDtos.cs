namespace LibraryManagementSystem.Application.DTOs;

public record AuditLogDto(
    Guid Id,
    string? UserId,
    string? UserName,
    string Action,
    string EntityName,
    string? EntityId,
    DateTime Timestamp,
    string? IpAddress
);