namespace LibraryManagementSystem.Application.DTOs;

public record SystemSettingDto(
    Guid Id,
    string Key,
    string? Value,
    string? Description
);