using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> GetTotalBooksAsync() => _context.Books.CountAsync(b => b.IsActive);
    
    public Task<int> GetTotalMembersAsync() => _context.Members.CountAsync(m => m.IsActive);
    
    public Task<int> GetActiveLoansCountAsync() => _context.Loans.CountAsync(l => l.Status == LoanStatus.Issued);
    
    public Task<int> GetOverdueLoansCountAsync() => _context.Loans.CountAsync(l => l.Status == LoanStatus.Issued && l.DueDate < DateTime.UtcNow);
    
    public Task<int> GetTodaysIssuesCountAsync() => _context.Loans.CountAsync(l => l.IssueDate.Date == DateTime.UtcNow.Date);
    
    public Task<int> GetTodaysReturnsCountAsync() => _context.Loans.CountAsync(l => l.ReturnDate.HasValue && l.ReturnDate.Value.Date == DateTime.UtcNow.Date);
    
    public Task<int> GetPendingReservationsCountAsync() => _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Pending);
    
    public async Task<decimal> GetOutstandingFinesTotalAsync()
    {
        return await _context.Fines
            .Where(f => f.PaymentStatus == FinePaymentStatus.Pending || f.PaymentStatus == FinePaymentStatus.PartiallyPaid)
            .SumAsync(f => f.RemainingAmount);
    }
}