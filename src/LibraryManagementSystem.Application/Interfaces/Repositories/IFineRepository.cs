using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IFineRepository : IGenericRepository<Fine> 
{
    Task<decimal> GetTotalUnpaidFinesByMemberAsync(Guid memberId);
}