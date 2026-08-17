using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<BookDto>> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetBookWithDetailsAsync(id);
        if (book == null) return ServiceResult<BookDto>.Failed("Book not found.");

        return ServiceResult<BookDto>.Succeeded(MapToDto(book));
    }

    public async Task<ServiceResult<IEnumerable<BookDto>>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var dtos = books.Select(MapToDto);
        return ServiceResult<IEnumerable<BookDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<PagedResult<BookDto>>> GetPagedBooksAsync(int page, int pageSize, string? searchTerm)
    {
        var books = await _bookRepository.GetAllAsync();
        
        var query = books.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchTerm) || b.ISBN.Contains(searchTerm));
        }

        var totalCount = query.Count();
        var pagedBooks = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        var pagedResult = new PagedResult<BookDto>
        {
            Items = pagedBooks.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };

        return ServiceResult<PagedResult<BookDto>>.Succeeded(pagedResult);
    }

    public async Task<ServiceResult<Book>> CreateBookAsync(Book book)
    {
        if (await _bookRepository.IsbnExistsAsync(book.ISBN))
            return ServiceResult<Book>.Failed("A book with this ISBN already exists.");

        await _bookRepository.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<Book>.Succeeded(book);
    }

    public async Task<ServiceResult<Book>> UpdateBookAsync(Book book)
    {
        if (await _bookRepository.IsbnExistsAsync(book.ISBN, book.Id))
            return ServiceResult<Book>.Failed("A book with this ISBN already exists.");

        book.UpdatedAt = DateTime.UtcNow;
        _bookRepository.Update(book);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<Book>.Succeeded(book);
    }

    public async Task<ServiceResult> DeleteBookAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) return ServiceResult.Failed("Book not found.");

        book.IsActive = false;
        book.UpdatedAt = DateTime.UtcNow;
        _bookRepository.Update(book);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto(
            book.Id,
            book.ISBN,
            book.Title,
            book.Subtitle,
            book.Author?.FirstName + " " + book.Author?.LastName,
            book.Category?.Name ?? "N/A",
            book.Publisher?.Name ?? "N/A",
            book.TotalCopies,
            book.AvailableCopies,
            book.IsActive,
            book.CoverImageUrl
        );
    }
}