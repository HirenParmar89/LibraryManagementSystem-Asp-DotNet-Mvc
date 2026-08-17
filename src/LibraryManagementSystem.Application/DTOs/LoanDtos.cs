using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record LoanDto(
    Guid Id,
    string BookTitle,
    string BookIsbn,
    string BookBarcode,
    string MemberName,
    DateTime IssueDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    LoanStatus Status,
    decimal FineAmount
);

public record IssueBookDto(
    Guid BookCopyId,
    Guid MemberId,
    string IssuedByUserId
);

public record ReturnBookDto(
    Guid LoanId,
    string ReturnedByUserId
);

public record RenewBookDto(
    Guid LoanId
);