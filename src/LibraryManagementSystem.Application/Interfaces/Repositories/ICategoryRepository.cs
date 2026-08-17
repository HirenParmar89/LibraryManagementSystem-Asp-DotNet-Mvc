using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category> 
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}