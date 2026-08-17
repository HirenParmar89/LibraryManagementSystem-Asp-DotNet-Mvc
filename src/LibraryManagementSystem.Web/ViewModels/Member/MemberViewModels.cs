using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Member;

public class MemberListViewModel
{
    public IEnumerable<MemberDto> Members { get; set; } = new List<MemberDto>();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
}

public class MemberFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(50)]
    public string MembershipNumber { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public MembershipType MembershipType { get; set; } = MembershipType.General;

    [Required]
    public int MaxBooksAllowed { get; set; } = 5;

    [DataType(DataType.Date)]
    public DateTime MembershipDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime MembershipExpiryDate { get; set; } = DateTime.Today.AddYears(1);

    public bool IsActive { get; set; } = true;
}

public class MemberDetailsViewModel
{
    public MemberDto Member { get; set; } = null!;
    // We will add Loans, Fines, and Reservations lists here in future phases
}