using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetBookWithDetailsAsync(Guid id);
    Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
}