using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    // GET: Members
    public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
    {
        var result = await _memberService.GetPagedMembersAsync(page, pageSize, searchTerm);
        
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new MemberListViewModel());
        }

        var viewModel = new MemberListViewModel
        {
            Members = result.Data.Items,
            PageNumber = result.Data.PageNumber,
            TotalPages = result.Data.TotalPages,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    // GET: Members/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _memberService.GetMemberByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var viewModel = new MemberDetailsViewModel
        {
            Member = result.Data
        };

        return View(viewModel);
    }

    // GET: Members/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public IActionResult Create()
    {
        var viewModel = new MemberFormViewModel
        {
            MembershipNumber = $"M{DateTime.Now:yyyyMMddHHmmss}" // Auto-generate a simple membership number
        };
        return View(viewModel);
    }

    // POST: Members/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Create(MemberFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var member = new Member
        {
            MembershipNumber = model.MembershipNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth,
            MembershipType = model.MembershipType,
            MaxBooksAllowed = model.MaxBooksAllowed,
            MembershipDate = model.MembershipDate,
            MembershipExpiryDate = model.MembershipExpiryDate,
            ApplicationUserId = Guid.NewGuid().ToString(), // Temporary: Link to actual Identity User later
            IsActive = true
        };

        var result = await _memberService.CreateMemberAsync(member);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Member created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Members/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _memberService.GetMemberByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var dto = result.Data;
        var viewModel = new MemberFormViewModel
        {
            Id = dto.Id,
            MembershipNumber = dto.MembershipNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            MembershipType = dto.MembershipType,
            MaxBooksAllowed = dto.MaxBooksAllowed,
            MembershipDate = dto.MembershipDate,
            MembershipExpiryDate = dto.MembershipExpiryDate,
            IsActive = dto.IsActive
        };

        return View(viewModel);
    }

    // POST: Members/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, MemberFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        // Fetch existing to preserve ApplicationUserId
        var existingMemberResult = await _memberService.GetMemberByIdAsync(id);
        if (!existingMemberResult.Success || existingMemberResult.Data == null) return NotFound();

        var member = new Member
        {
            Id = model.Id,
            MembershipNumber = model.MembershipNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth,
            MembershipType = model.MembershipType,
            MaxBooksAllowed = model.MaxBooksAllowed,
            MembershipDate = model.MembershipDate,
            MembershipExpiryDate = model.MembershipExpiryDate,
            ApplicationUserId = existingMemberResult.Data.ApplicationUserId ?? Guid.NewGuid().ToString(),
            IsActive = model.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _memberService.UpdateMemberAsync(member);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Member updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Members/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _memberService.DeactivateMemberAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Member deactivated successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}