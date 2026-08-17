using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Web.ViewModels.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IMemberService _memberService;
    private readonly IBookCopyService _bookCopyService;
    private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

    public LoansController(
        ILoanService loanService, 
        IMemberService memberService, 
        IBookCopyService bookCopyService,
        UserManager<Infrastructure.Identity.ApplicationUser> userManager)
    {
        _loanService = loanService;
        _memberService = memberService;
        _bookCopyService = bookCopyService;
        _userManager = userManager;
    }

    // GET: Loans/Issue
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Issue()
    {
        var membersResult = await _memberService.GetPagedMembersAsync(1, 1000, null);
        var copiesResult = await _bookCopyService.GetAllCopiesAsync();

        ViewBag.Members = membersResult.Data?.Items ?? new List<MemberDto>();
        ViewBag.AvailableCopies = (copiesResult.Data ?? new List<BookCopyDto>()).Where(c => c.IsAvailable);

        return View(new IssueBookViewModel());
    }

    // POST: Loans/Issue
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Issue(IssueBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var membersResult = await _memberService.GetPagedMembersAsync(1, 1000, null);
            var copiesResult = await _bookCopyService.GetAllCopiesAsync();
            ViewBag.Members = membersResult.Data?.Items ?? new List<MemberDto>();
            ViewBag.AvailableCopies = (copiesResult.Data ?? new List<BookCopyDto>()).Where(c => c.IsAvailable);
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var issueDto = new IssueBookDto(
            model.BookCopyId,
            model.MemberId,
            user?.Id ?? "System"
        );

        var result = await _loanService.IssueBookAsync(issueDto);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to issue book.";
        }
        else
        {
            TempData["SuccessMessage"] = "Book issued successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Loans (Active Loans)
    public async Task<IActionResult> Index()
    {
        var result = await _loanService.GetActiveLoansAsync();
        var viewModel = new LoanListViewModel
        {
            Loans = result.Data ?? new List<LoanDto>()
        };
        return View(viewModel);
    }

    // GET: Loans/History
    public async Task<IActionResult> History()
    {
        var result = await _loanService.GetLoanHistoryAsync();
        var viewModel = new LoanListViewModel
        {
            Loans = result.Data ?? new List<LoanDto>()
        };
        return View(viewModel);
    }

    // POST: Loans/Return/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Return(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        var returnDto = new ReturnBookDto(id, user?.Id ?? "System");

        var result = await _loanService.ReturnBookAsync(returnDto);
        
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to return book.";
        }
        else
        {
            TempData["SuccessMessage"] = "Book returned successfully.";
            if (result.Data?.FineAmount > 0)
            {
                TempData["InfoMessage"] = $"A fine of {result.Data.FineAmount:C} was generated for late return.";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Loans/Renew/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant,Member")]
    public async Task<IActionResult> Renew(Guid id)
    {
        var renewDto = new RenewBookDto(id);
        var result = await _loanService.RenewBookAsync(renewDto);
        
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to renew book.";
        }
        else
        {
            TempData["SuccessMessage"] = "Book renewed successfully. New due date: " + result.Data?.DueDate.ToString("yyyy-MM-dd");
        }

        return RedirectToAction(nameof(Index));
    }
    // GET: /Loans/GetCopyByBarcode?barcode=BAR123
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<JsonResult> GetCopyByBarcode(string barcode)
    {
        var result = await _bookCopyService.GetCopyByBarcodeAsync(barcode);
        
        if (result.Success && result.Data != null)
        {
            return Json(new { 
                success = true, 
                copyId = result.Data.Id, 
                bookTitle = result.Data.BookTitle,
                status = result.Data.Status.ToString(),
                isAvailable = result.Data.IsAvailable
            });
        }
        
        return Json(new { success = false, message = result.ErrorMessage ?? "Book copy not found." });
    }
}