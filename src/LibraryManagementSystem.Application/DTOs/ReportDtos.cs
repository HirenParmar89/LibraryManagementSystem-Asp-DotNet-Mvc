using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record CirculationReportDto(
    Guid LoanId,
    string BookTitle,
    string MemberName,
    DateTime IssueDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    LoanStatus Status,
    decimal FineAmount
);

public record InventoryReportDto(
    string ISBN,
    string Title,
    string Author,
    string Category,
    int TotalCopies,
    int AvailableCopies
);

public record MemberReportDto(
    string MembershipNumber,
    string FullName,
    string Email,
    DateTime MembershipDate,
    DateTime ExpiryDate,
    bool IsActive
);

public record FineReportDto(
    Guid FineId,
    string MemberName,
    decimal Amount,
    decimal PaidAmount,
    decimal RemainingAmount,
    FinePaymentStatus Status,
    DateTime FineDate
);