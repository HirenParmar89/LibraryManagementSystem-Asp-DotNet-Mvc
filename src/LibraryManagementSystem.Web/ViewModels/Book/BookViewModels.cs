using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LibraryManagementSystem.Web.ViewModels.Book;

public class BookListViewModel
{
    public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
}

public class BookFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }
    public string? Description { get; set; }

    [Required]
    public Guid AuthorId { get; set; }
    [Required]
    public Guid CategoryId { get; set; }
    [Required]
    public Guid PublisherId { get; set; }

    [Required]
    public DateTime PublishedDate { get; set; } = DateTime.Today;

    public string? Edition { get; set; }
    public string? Language { get; set; } = "English";
    public int PageCount { get; set; }
    public decimal Price { get; set; }
    public string? ShelfLocation { get; set; }
    
    public string? CoverImageUrl { get; set; } // Existing property for URL

    [Display(Name = "Cover Image")]
    public IFormFile? CoverImage { get; set; } // New property for file upload

    [Required]
    public int TotalCopies { get; set; } = 1;
    
    public int AvailableCopies { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    // Dropdown data
    public IEnumerable<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public IEnumerable<PublisherDto> Publishers { get; set; } = new List<PublisherDto>();
}

public class BookDetailsViewModel
{
    public BookDto Book { get; set; } = null!;
    public IEnumerable<BookCopyDto> Copies { get; set; } = new List<BookCopyDto>();
}