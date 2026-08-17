using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.BookCopy;

public class BookCopyListViewModel
{
    public IEnumerable<BookCopyDto> Copies { get; set; } = new List<BookCopyDto>();
    public Guid? FilterBookId { get; set; }
    public string? FilterBookTitle { get; set; }
}

public class BookCopyFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid BookId { get; set; }

    [Required, StringLength(50)]
    public string AccessionNumber { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    public BookCondition Condition { get; set; } = BookCondition.New;

    [Required]
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;

    public DateTime? PurchaseDate { get; set; }
    public decimal? Price { get; set; }
    public string? ShelfLocation { get; set; }

    // Dropdown data
    public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>();
    
    // For display purposes
    public string? BookTitle { get; set; }
}