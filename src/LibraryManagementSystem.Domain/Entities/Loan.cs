using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Loan : AuditableEntity
{
    public Guid BookCopyId { get; set; }
    public BookCopy? BookCopy { get; set; }
    
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
    
    public string IssuedByUserId { get; set; } = string.Empty; // Librarian/Staff who issued the book
    
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public LoanStatus Status { get; set; } = LoanStatus.Issued;
    public int RenewalCount { get; set; } = 0;
    
    public decimal FineAmount { get; set; } = 0m;
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}