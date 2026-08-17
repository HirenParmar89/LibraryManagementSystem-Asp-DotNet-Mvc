using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Web.ViewModels.BookCopy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class BookCopiesController : Controller
{
    private readonly IBookCopyService _bookCopyService;
    private readonly IBookService _bookService;

    public BookCopiesController(IBookCopyService bookCopyService, IBookService bookService)
    {
        _bookCopyService = bookCopyService;
        _bookService = bookService;
    }

    // GET: BookCopies
    public async Task<IActionResult> Index(Guid? bookId)
    {
        ServiceResult<IEnumerable<BookCopyDto>> result;
        
        if (bookId.HasValue)
        {
            result = await _bookCopyService.GetCopiesByBookAsync(bookId.Value);
            var bookResult = await _bookService.GetBookByIdAsync(bookId.Value);
            ViewBag.BookTitle = bookResult.Data?.Title;
        }
        else
        {
            result = await _bookCopyService.GetAllCopiesAsync();
        }

        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new BookCopyListViewModel());
        }

        var viewModel = new BookCopyListViewModel
        {
            Copies = result.Data,
            FilterBookId = bookId
        };

        return View(viewModel);
    }

    // GET: BookCopies/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(Guid? bookId)
    {
        var viewModel = new BookCopyFormViewModel
        {
            BookId = bookId ?? Guid.Empty,
            Status = BookCopyStatus.Available,
            Condition = BookCondition.New
        };

        await PopulateBooksDropdown(viewModel);
        return View(viewModel);
    }

    // POST: BookCopies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBooksDropdown(model);
            return View(model);
        }

        var copy = new BookCopy
        {
            BookId = model.BookId,
            AccessionNumber = model.AccessionNumber,
            Barcode = model.Barcode,
            Condition = model.Condition,
            Status = model.Status,
            PurchaseDate = model.PurchaseDate,
            Price = model.Price,
            ShelfLocation = model.ShelfLocation,
            IsAvailable = model.Status == BookCopyStatus.Available
        };

        var result = await _bookCopyService.CreateCopyAsync(copy);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            await PopulateBooksDropdown(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Book copy added successfully.";
        return RedirectToAction(nameof(Index), new { bookId = model.BookId });
    }

    // GET: BookCopies/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var copyResult = await _bookCopyService.GetCopyByIdAsync(id);
        if (!copyResult.Success || copyResult.Data == null) return NotFound();

        var dto = copyResult.Data;
        var viewModel = new BookCopyFormViewModel
        {
            Id = dto.Id,
            BookId = dto.BookId,
            AccessionNumber = dto.AccessionNumber,
            Barcode = dto.Barcode,
            Condition = dto.Condition,
            Status = dto.Status,
            BookTitle = dto.BookTitle
        };

        await PopulateBooksDropdown(viewModel);
        return View(viewModel);
    }

    // POST: BookCopies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, BookCopyFormViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateBooksDropdown(model);
            return View(model);
        }

        var copy = new BookCopy
        {
            Id = model.Id,
            BookId = model.BookId,
            AccessionNumber = model.AccessionNumber,
            Barcode = model.Barcode,
            Condition = model.Condition,
            Status = model.Status,
            PurchaseDate = model.PurchaseDate,
            Price = model.Price,
            ShelfLocation = model.ShelfLocation,
            IsAvailable = model.Status == BookCopyStatus.Available
        };

        var result = await _bookCopyService.UpdateCopyAsync(copy);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred.");
            await PopulateBooksDropdown(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Book copy updated successfully.";
        return RedirectToAction(nameof(Index), new { bookId = model.BookId });
    }

    // POST: BookCopies/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookCopyService.DeleteCopyAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Book copy deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateBooksDropdown(BookCopyFormViewModel model)
    {
        var booksResult = await _bookService.GetAllBooksAsync();
        model.Books = booksResult.Data ?? new List<BookDto>();
    }
}