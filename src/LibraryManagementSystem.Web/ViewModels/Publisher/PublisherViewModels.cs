using LibraryManagementSystem.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Publisher;

public class PublisherListViewModel
{
    public IEnumerable<PublisherDto> Publishers { get; set; } = new List<PublisherDto>();
    public string? SearchTerm { get; set; }
}

public class PublisherFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}

public class PublisherDetailsViewModel
{
    public PublisherDto Publisher { get; set; } = null!;
}