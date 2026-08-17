namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<int> GetTotalBooksAsync();
    Task<int> GetTotalMembersAsync();
    Task<int> GetActiveLoansCountAsync();
    Task<int> GetOverdueLoansCountAsync();
    Task<int> GetTodaysIssuesCountAsync();
    Task<int> GetTodaysReturnsCountAsync();
    Task<int> GetPendingReservationsCountAsync();
    Task<decimal> GetOutstandingFinesTotalAsync();
}