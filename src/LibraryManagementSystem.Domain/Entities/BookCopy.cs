using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class BookCopy : BaseEntity
{
    public Guid BookId { get; set; }
    public Book? Book { get; set; }
    
    public string AccessionNumber { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    
    public BookCondition Condition { get; set; } = BookCondition.New;
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
    
    public DateTime? PurchaseDate { get; set; }
    public decimal? Price { get; set; }
    public string? ShelfLocation { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}