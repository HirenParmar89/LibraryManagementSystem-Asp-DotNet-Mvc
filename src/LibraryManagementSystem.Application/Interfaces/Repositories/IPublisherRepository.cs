using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IPublisherRepository : IGenericRepository<Publisher> 
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}