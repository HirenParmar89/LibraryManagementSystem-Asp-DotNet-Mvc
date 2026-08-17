using LibraryManagementSystem.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Category;

public class CategoryListViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public string? SearchTerm { get; set; }
}

public class CategoryFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    // Dropdown data for Parent Category
    public IEnumerable<CategoryDto> ParentCategories { get; set; } = new List<CategoryDto>();
}

public class CategoryDetailsViewModel
{
    public CategoryDto Category { get; set; } = null!;
}