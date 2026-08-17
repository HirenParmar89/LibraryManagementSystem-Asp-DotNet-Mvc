using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record FineDto(
    Guid Id,
    Guid LoanId,
    Guid MemberId,
    string MemberName,
    decimal Amount,
    decimal PaidAmount,
    decimal RemainingAmount,
    FinePaymentStatus PaymentStatus,
    DateTime FineDate
);

public record FinePaymentDto(
    Guid FineId,
    decimal Amount,
    PaymentMethod Method,
    string ReceivedByUserId
);