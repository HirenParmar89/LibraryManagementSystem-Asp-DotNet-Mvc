using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Reservation : AuditableEntity
{
    public Guid BookId { get; set; }
    public Book? Book { get; set; }
    
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
    
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public int QueuePosition { get; set; }
    
    public string? Notes { get; set; }
}