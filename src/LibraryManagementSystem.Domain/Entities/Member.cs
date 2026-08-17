using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Member : AuditableEntity
{
    public string MembershipNumber { get; set; } = string.Empty;
    public string ApplicationUserId { get; set; } = string.Empty; // Linked to IdentityUser in Phase 8
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
    public DateTime MembershipExpiryDate { get; set; }
    
    public MembershipType MembershipType { get; set; } = MembershipType.General;
    public int MaxBooksAllowed { get; set; } = 5;
    
    public bool IsActive { get; set; } = true;
    public string? ProfileImageUrl { get; set; }

    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}