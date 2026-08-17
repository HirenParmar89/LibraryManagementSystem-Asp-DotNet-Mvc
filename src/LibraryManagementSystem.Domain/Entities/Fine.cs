using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine : BaseEntity
{
    public Guid LoanId { get; set; }
    public Loan? Loan { get; set; }
    
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
    
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FineDate { get; set; } = DateTime.UtcNow;
    
    public decimal PaidAmount { get; set; } = 0m;
    public decimal RemainingAmount { get; set; }
    
    public FinePaymentStatus PaymentStatus { get; set; } = FinePaymentStatus.Pending;
    public DateTime? PaidDate { get; set; }
    
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<FinePayment> FinePayments { get; set; } = new List<FinePayment>();
}