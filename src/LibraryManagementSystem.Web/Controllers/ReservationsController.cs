using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Web.ViewModels.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;

    public ReservationsController(
        IReservationService reservationService, 
        IBookService bookService, 
        IMemberService memberService)
    {
        _reservationService = reservationService;
        _bookService = bookService;
        _memberService = memberService;
    }

    // GET: Reservations
    public async Task<IActionResult> Index()
    {
        var result = await _reservationService.GetAllReservationsAsync();
        var viewModel = new ReservationListViewModel
        {
            Reservations = result.Data ?? new List<ReservationDto>()
        };
        return View(viewModel);
    }

    // GET: Reservations/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant,Member")]
    public async Task<IActionResult> Create()
    {
        var booksResult = await _bookService.GetAllBooksAsync();
        var membersResult = await _memberService.GetPagedMembersAsync(1, 1000, null);

        var viewModel = new CreateReservationViewModel
        {
            Books = booksResult.Data ?? new List<BookDto>(),
            Members = membersResult.Data?.Items ?? new List<MemberDto>()
        };
        
        return View(viewModel);
    }

    // POST: Reservations/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant,Member")]
    public async Task<IActionResult> Create(CreateReservationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var booksResult = await _bookService.GetAllBooksAsync();
            var membersResult = await _memberService.GetPagedMembersAsync(1, 1000, null);
            model.Books = booksResult.Data ?? new List<BookDto>();
            model.Members = membersResult.Data?.Items ?? new List<MemberDto>();
            return View(model);
        }

        var result = await _reservationService.CreateReservationAsync(model.BookId, model.MemberId);
        
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to create reservation.";
        }
        else
        {
            TempData["SuccessMessage"] = "Reservation created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Reservations/Cancel/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant,Member")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _reservationService.CancelReservationAsync(id);
        
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to cancel reservation.";
        }
        else
        {
            TempData["SuccessMessage"] = "Reservation cancelled successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Reservations/Fulfill/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian,Assistant")]
    public async Task<IActionResult> Fulfill(Guid id)
    {
        var result = await _reservationService.FulfillReservationAsync(id);
        
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to fulfill reservation.";
        }
        else
        {
            TempData["SuccessMessage"] = "Reservation fulfilled successfully. Book issued to member.";
        }

        return RedirectToAction(nameof(Index));
    }
}