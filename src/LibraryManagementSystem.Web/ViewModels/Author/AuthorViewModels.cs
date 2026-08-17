using LibraryManagementSystem.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Author;

public class AuthorListViewModel
{
    public IEnumerable<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    public string? SearchTerm { get; set; }
}

public class AuthorFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public string? Biography { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    public string? Country { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? Website { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class AuthorDetailsViewModel
{
    public AuthorDto Author { get; set; } = null!;
}