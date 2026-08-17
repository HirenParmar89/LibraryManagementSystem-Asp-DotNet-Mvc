using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IAuthorRepository : IGenericRepository<Author> 
{
    Task<bool> NameExistsAsync(string firstName, string lastName, Guid? excludeId = null);
}