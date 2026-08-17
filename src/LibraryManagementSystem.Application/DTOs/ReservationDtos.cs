using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record ReservationDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    Guid MemberId,
    string MemberName,
    DateTime ReservationDate,
    DateTime? ExpiryDate,
    ReservationStatus Status,
    int QueuePosition
);

public record CreateReservationDto(
    Guid BookId,
    Guid MemberId
);