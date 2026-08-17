using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Services;

public interface IBookService
{
    Task<ServiceResult<BookDto>> GetBookByIdAsync(Guid id);
    Task<ServiceResult<IEnumerable<BookDto>>> GetAllBooksAsync();
    Task<ServiceResult<PagedResult<BookDto>>> GetPagedBooksAsync(int page, int pageSize, string? searchTerm);
    Task<ServiceResult<Book>> CreateBookAsync(Book book);
    Task<ServiceResult<Book>> UpdateBookAsync(Book book);
    Task<ServiceResult> DeleteBookAsync(Guid id);
}