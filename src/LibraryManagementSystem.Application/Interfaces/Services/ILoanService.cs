using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;

namespace LibraryManagementSystem.Application.Interfaces.Services;

public interface ILoanService
{
    Task<ServiceResult<LoanDto>> IssueBookAsync(IssueBookDto dto);
    Task<ServiceResult<LoanDto>> ReturnBookAsync(ReturnBookDto dto);
    Task<ServiceResult<LoanDto>> RenewBookAsync(RenewBookDto dto);
    
    Task<ServiceResult<IEnumerable<LoanDto>>> GetActiveLoansAsync();
    Task<ServiceResult<IEnumerable<LoanDto>>> GetOverdueLoansAsync();
    Task<ServiceResult<IEnumerable<LoanDto>>> GetLoanHistoryAsync();
}