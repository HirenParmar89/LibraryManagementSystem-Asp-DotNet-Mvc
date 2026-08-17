using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class AuthorsController : Controller
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    // GET: Authors
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var result = await _authorService.GetAllAuthorsAsync();
        
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new AuthorListViewModel());
        }

        var authors = result.Data.ToList();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            authors = authors.Where(a => 
                a.FirstName.ToLower().Contains(searchTerm) || 
                a.LastName.ToLower().Contains(searchTerm)).ToList();
        }

        var viewModel = new AuthorListViewModel
        {
            Authors = authors,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    // GET: Authors/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _authorService.GetAuthorByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new AuthorDetailsViewModel
        {
            Author = result.Data
        };

        return View(viewModel);
    }

    // GET: Authors/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public IActionResult Create()
    {
        return View(new AuthorFormViewModel());
    }

    // POST: Authors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var author = new Author
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Biography = model.Biography,
            DateOfBirth = model.DateOfBirth,
            Country = model.Country,
            Email = model.Email,
            Website = model.Website,
            IsActive = true
        };

        var result = await _authorService.CreateAuthorAsync(author);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Author created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Authors/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _authorService.GetAuthorByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        // We need the full entity to edit, but our service returns a DTO. 
        // Let's fetch the entity directly or expand the DTO. For now, we map from DTO.
        // Note: For a production app, you'd have a GetAuthorForEditAsync method.
        var author = new Author
        {
            Id = result.Data.Id,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName
        };

        var viewModel = new AuthorFormViewModel
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        };

        return View(viewModel);
    }

    // POST: Authors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, AuthorFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var author = new Author
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Biography = model.Biography,
            DateOfBirth = model.DateOfBirth,
            Country = model.Country,
            Email = model.Email,
            Website = model.Website,
            IsActive = model.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _authorService.UpdateAuthorAsync(author);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Author updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Authors/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _authorService.DeleteAuthorAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Author deactivated successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}