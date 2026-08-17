using LibraryManagementSystem.Domain.Common;

namespace LibraryManagementSystem.Domain.Entities;

public class Book : AuditableEntity
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public Guid PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
    
    public DateTime PublishedDate { get; set; }
    public string? Edition { get; set; }
    public string? Language { get; set; }
    public int PageCount { get; set; }
    public decimal Price { get; set; }
    public string? ShelfLocation { get; set; }
    public string? CoverImageUrl { get; set; }
    
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}