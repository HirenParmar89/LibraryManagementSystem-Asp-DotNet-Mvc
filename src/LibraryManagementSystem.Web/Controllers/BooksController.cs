using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;
    private readonly IPublisherService _publisherService;
    private readonly IFileService _fileService;

    public BooksController(
        IBookService bookService, 
        IAuthorService authorService, 
        ICategoryService categoryService, 
        IPublisherService publisherService,
        IFileService fileService)
    {
        _bookService = bookService;
        _authorService = authorService;
        _categoryService = categoryService;
        _publisherService = publisherService;
        _fileService = fileService;
    }

    // GET: Books
    public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
    {
        var result = await _bookService.GetPagedBooksAsync(page, pageSize, searchTerm);
        
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
            return View(new BookListViewModel());
        }

        var viewModel = new BookListViewModel
        {
            Books = result.Data.Items,
            PageNumber = result.Data.PageNumber,
            TotalPages = result.Data.TotalPages,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var bookResult = await _bookService.GetBookByIdAsync(id);
        if (!bookResult.Success || bookResult.Data == null)
        {
            return NotFound();
        }

        var viewModel = new BookDetailsViewModel
        {
            Book = bookResult.Data,
            Copies = new List<BookCopyDto>() 
        };

        return View(viewModel);
    }

    // GET: Books/Create
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new BookFormViewModel();
        await PopulateDropdowns(viewModel);
        return View(viewModel);
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(model);
            return View(model);
        }

        string? imageUrl = null;
        if (model.CoverImage != null)
        {
            try
            {
                using var stream = model.CoverImage.OpenReadStream();
                imageUrl = await _fileService.SaveFileAsync(stream, model.CoverImage.FileName, new[] { ".jpg", ".jpeg", ".png" }, "books");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("CoverImage", ex.Message);
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        var book = new Book
        {
            ISBN = model.ISBN,
            Title = model.Title,
            Subtitle = model.Subtitle,
            Description = model.Description,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId,
            PublishedDate = model.PublishedDate,
            Edition = model.Edition,
            Language = model.Language,
            PageCount = model.PageCount,
            Price = model.Price,
            ShelfLocation = model.ShelfLocation,
            CoverImageUrl = imageUrl,
            TotalCopies = model.TotalCopies,
            AvailableCopies = model.TotalCopies,
            IsActive = true
        };

        var result = await _bookService.CreateBookAsync(book);
        
        if (!result.Success)
        {
            if (imageUrl != null) _fileService.DeleteFile(imageUrl);
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred while creating the book.");
            await PopulateDropdowns(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Book created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Books/Edit/5
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var bookResult = await _bookService.GetBookByIdAsync(id);
        if (!bookResult.Success || bookResult.Data == null)
        {
            return NotFound();
        }

        var book = new Book
        {
            Id = bookResult.Data.Id,
            ISBN = bookResult.Data.ISBN,
            Title = bookResult.Data.Title,
            TotalCopies = bookResult.Data.TotalCopies,
            AvailableCopies = bookResult.Data.AvailableCopies,
            IsActive = bookResult.Data.IsActive,
            CoverImageUrl = bookResult.Data.CoverImageUrl
        };

        var viewModel = new BookFormViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            IsActive = book.IsActive,
            CoverImageUrl = book.CoverImageUrl
        };

        await PopulateDropdowns(viewModel);
        return View(viewModel);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin,Librarian")]
    public async Task<IActionResult> Edit(Guid id, BookFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(model);
            return View(model);
        }

        var existingBookResult = await _bookService.GetBookByIdAsync(id);
        if (!existingBookResult.Success || existingBookResult.Data == null) return NotFound();

        string? imageUrl = existingBookResult.Data.CoverImageUrl; 
        if (model.CoverImage != null)
        {
            try
            {
                using var stream = model.CoverImage.OpenReadStream();
                imageUrl = await _fileService.SaveFileAsync(stream, model.CoverImage.FileName, new[] { ".jpg", ".jpeg", ".png" }, "books");
                if (!string.IsNullOrEmpty(existingBookResult.Data.CoverImageUrl))
                {
                    _fileService.DeleteFile(existingBookResult.Data.CoverImageUrl);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("CoverImage", ex.Message);
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        var book = new Book
        {
            Id = model.Id,
            ISBN = model.ISBN,
            Title = model.Title,
            Subtitle = model.Subtitle,
            Description = model.Description,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId,
            PublishedDate = model.PublishedDate,
            Edition = model.Edition,
            Language = model.Language,
            PageCount = model.PageCount,
            Price = model.Price,
            ShelfLocation = model.ShelfLocation,
            CoverImageUrl = imageUrl,
            TotalCopies = model.TotalCopies,
            AvailableCopies = model.AvailableCopies,
            IsActive = model.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _bookService.UpdateBookAsync(book);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "An error occurred while updating the book.");
            await PopulateDropdowns(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Book updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Books/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred.";
        }
        else
        {
            TempData["SuccessMessage"] = "Book deactivated successfully.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(BookFormViewModel model)
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();
        var publishers = await _publisherService.GetAllPublishersAsync();

        model.Authors = authors.Data ?? new List<AuthorDto>();
        model.Categories = categories.Data ?? new List<CategoryDto>();
        model.Publishers = publishers.Data ?? new List<PublisherDto>();
    }
}