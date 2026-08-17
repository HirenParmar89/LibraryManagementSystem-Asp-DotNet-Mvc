using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Web.ViewModels.Fine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class FinesController : Controller
{
    private readonly IFineService _fineService;
    private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

    public FinesController(IFineService fineService, UserManager<Infrastructure.Identity.ApplicationUser> userManager)
    {
        _fineService = fineService;
        _userManager = userManager;
    }

    // GET: Fines
    public async Task<IActionResult> Index()
    {
        var result = await _fineService.GetAllFinesAsync();
        var viewModel = new FineListViewModel
        {
            Fines = result.Data ?? new List<FineDto>()
        };
        return View(viewModel);
    }

    // GET: Fines/Payment/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Payment(Guid id)
    {
        var result = await _fineService.GetFineByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var dto = result.Data;
        var viewModel = new FinePaymentViewModel
        {
            FineId = dto.Id,
            MemberName = dto.MemberName,
            TotalAmount = dto.Amount,
            PaidAmount = dto.PaidAmount,
            RemainingAmount = dto.RemainingAmount,
            PaymentAmount = dto.RemainingAmount // Default to full payment
        };

        return View(viewModel);
    }

    // POST: Fines/Payment/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Payment(FinePaymentViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        var paymentDto = new FinePaymentDto(
            model.FineId,
            model.PaymentAmount,
            model.PaymentMethod,
            user?.Id ?? "System"
        );

        var result = await _fineService.RecordPaymentAsync(paymentDto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Failed to record payment.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Payment recorded successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Fines/Waive/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Waive(Guid id)
    {
        var result = await _fineService.WaiveFineAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to waive fine.";
        }
        else
        {
            TempData["SuccessMessage"] = "Fine waived successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}