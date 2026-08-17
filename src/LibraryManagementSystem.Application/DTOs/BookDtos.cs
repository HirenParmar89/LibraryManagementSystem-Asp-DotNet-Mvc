using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record BookDto(
    Guid Id,
    string ISBN,
    string Title,
    string? Subtitle,
    string AuthorName,
    string CategoryName,
    string PublisherName,
    int TotalCopies,
    int AvailableCopies,
    bool IsActive,
    string? CoverImageUrl // Added this property
);

public record BookCopyDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    string AccessionNumber,
    string Barcode,
    BookCondition Condition,
    BookCopyStatus Status,
    bool IsAvailable
);

// Added for dropdowns in Phase 14
public record AuthorDto(Guid Id, string FirstName, string LastName);
public record CategoryDto(Guid Id, string Name);
public record PublisherDto(Guid Id, string Name);