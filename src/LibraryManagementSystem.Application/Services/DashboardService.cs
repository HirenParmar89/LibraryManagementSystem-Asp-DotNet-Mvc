using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Application.ViewModels;

namespace LibraryManagementSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<ServiceResult<DashboardViewModel>> GetDashboardDataAsync()
    {
        var model = new DashboardViewModel
        {
            TotalBooks = await _dashboardRepository.GetTotalBooksAsync(),
            TotalMembers = await _dashboardRepository.GetTotalMembersAsync(),
            IssuedCopies = await _dashboardRepository.GetActiveLoansCountAsync(),
            OverdueLoans = await _dashboardRepository.GetOverdueLoansCountAsync(),
            TodaysIssues = await _dashboardRepository.GetTodaysIssuesCountAsync(),
            TodaysReturns = await _dashboardRepository.GetTodaysReturnsCountAsync(),
            PendingReservations = await _dashboardRepository.GetPendingReservationsCountAsync(),
            OutstandingFines = await _dashboardRepository.GetOutstandingFinesTotalAsync()
        };

        return ServiceResult<DashboardViewModel>.Succeeded(model);
    }
}