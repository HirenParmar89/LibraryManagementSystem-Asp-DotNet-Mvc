using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<Loan?> GetLoanWithDetailsAsync(Guid id);
    Task<IEnumerable<Loan>> GetAllLoansWithDetailsAsync();
    Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId);
    Task<int> GetActiveLoanCountByMemberAsync(Guid memberId);
}