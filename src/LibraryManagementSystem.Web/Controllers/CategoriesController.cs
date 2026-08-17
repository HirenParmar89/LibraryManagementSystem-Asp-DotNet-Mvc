using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: Categories
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new CategoryListViewModel());
        }

        var categories = result.Data.ToList();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            categories = categories.Where(c => c.Name.ToLower().Contains(searchTerm)).ToList();
        }

        var viewModel = new CategoryListViewModel
        {
            Categories = categories,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new CategoryDetailsViewModel
        {
            Category = result.Data
        };

        return View(viewModel);
    }

    // GET: Categories/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CategoryFormViewModel();
        await PopulateParentCategoriesDropdown(viewModel);
        return View(viewModel);
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateParentCategoriesDropdown(model);
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            ParentCategoryId = model.ParentCategoryId,
            IsActive = true
        };

        var result = await _categoryService.CreateCategoryAsync(category);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            await PopulateParentCategoriesDropdown(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Category created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Categories/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new CategoryFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name
        };

        await PopulateParentCategoriesDropdown(viewModel);
        return View(viewModel);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, CategoryFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await PopulateParentCategoriesDropdown(model);
            return View(model);
        }

        var category = new Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            ParentCategoryId = model.ParentCategoryId,
            IsActive = model.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _categoryService.UpdateCategoryAsync(category);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            await PopulateParentCategoriesDropdown(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Category updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Categories/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Category deactivated successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateParentCategoriesDropdown(CategoryFormViewModel model)
    {
        var categoriesResult = await _categoryService.GetAllCategoriesAsync();
        // Exclude the current category from being its own parent
        model.ParentCategories = (categoriesResult.Data ?? new List<CategoryDto>())
            .Where(c => c.Id != model.Id);
    }
}