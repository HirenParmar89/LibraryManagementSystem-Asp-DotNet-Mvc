using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs;

public record MemberDto(
    Guid Id,
    string MembershipNumber,
    string? ApplicationUserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Address,
    DateTime? DateOfBirth,
    MembershipType MembershipType,
    int MaxBooksAllowed,
    DateTime MembershipDate,
    DateTime MembershipExpiryDate,
    bool IsActive
);