using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class PublishersController : Controller
{
    private readonly IPublisherService _publisherService;

    public PublishersController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    // GET: Publishers
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var result = await _publisherService.GetAllPublishersAsync();
        
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new PublisherListViewModel());
        }

        var publishers = result.Data.ToList();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            publishers = publishers.Where(p => p.Name.ToLower().Contains(searchTerm)).ToList();
        }

        var viewModel = new PublisherListViewModel
        {
            Publishers = publishers,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    // GET: Publishers/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _publisherService.GetPublisherByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new PublisherDetailsViewModel
        {
            Publisher = result.Data
        };

        return View(viewModel);
    }

    // GET: Publishers/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public IActionResult Create()
    {
        return View(new PublisherFormViewModel());
    }

    // POST: Publishers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(PublisherFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var publisher = new Publisher
        {
            Name = model.Name,
            Description = model.Description,
            Email = model.Email,
            Phone = model.Phone,
            Website = model.Website,
            Address = model.Address,
            IsActive = true
        };

        var result = await _publisherService.CreatePublisherAsync(publisher);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Publisher created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Publishers/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _publisherService.GetPublisherByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new PublisherFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name
        };

        return View(viewModel);
    }

    // POST: Publishers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, PublisherFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var publisher = new Publisher
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Email = model.Email,
            Phone = model.Phone,
            Website = model.Website,
            Address = model.Address,
            IsActive = model.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _publisherService.UpdatePublisherAsync(publisher);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Publisher updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Publishers/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _publisherService.DeletePublisherAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Publisher deactivated successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}