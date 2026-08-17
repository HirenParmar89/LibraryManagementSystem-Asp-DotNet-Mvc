using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;

namespace LibraryManagementSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IFineRepository _fineRepository;

    public ReportService(
        ILoanRepository loanRepository, 
        IBookRepository bookRepository, 
        IMemberRepository memberRepository, 
        IFineRepository fineRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _fineRepository = fineRepository;
    }

    public async Task<ServiceResult<IEnumerable<CirculationReportDto>>> GetCirculationReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var loans = await _loanRepository.GetAllLoansWithDetailsAsync();

        if (startDate.HasValue)
            loans = loans.Where(l => l.IssueDate.Date >= startDate.Value.Date).ToList();
        
        if (endDate.HasValue)
            loans = loans.Where(l => l.IssueDate.Date <= endDate.Value.Date).ToList();

        var dtos = loans.Select(l => new CirculationReportDto(
            l.Id,
            l.BookCopy?.Book?.Title ?? "Unknown",
            l.Member != null ? $"{l.Member.FirstName} {l.Member.LastName}" : "Unknown",
            l.IssueDate,
            l.DueDate,
            l.ReturnDate,
            l.Status,
            l.FineAmount
        ));

        return ServiceResult<IEnumerable<CirculationReportDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<IEnumerable<InventoryReportDto>>> GetInventoryReportAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        
        var dtos = books.Select(b => new InventoryReportDto(
            b.ISBN,
            b.Title,
            b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
            b.Category?.Name ?? "Unknown",
            b.TotalCopies,
            b.AvailableCopies
        ));

        return ServiceResult<IEnumerable<InventoryReportDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<IEnumerable<MemberReportDto>>> GetMemberReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var members = await _memberRepository.GetAllAsync();

        if (startDate.HasValue)
            members = members.Where(m => m.MembershipDate.Date >= startDate.Value.Date).ToList();
        
        if (endDate.HasValue)
            members = members.Where(m => m.MembershipDate.Date <= endDate.Value.Date).ToList();

        var dtos = members.Select(m => new MemberReportDto(
            m.MembershipNumber,
            $"{m.FirstName} {m.LastName}",
            m.Email,
            m.MembershipDate,
            m.MembershipExpiryDate,
            m.IsActive
        ));

        return ServiceResult<IEnumerable<MemberReportDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<IEnumerable<FineReportDto>>> GetFineReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var fines = await _fineRepository.GetAllAsync();
        var members = await _memberRepository.GetAllAsync();

        if (startDate.HasValue)
            fines = fines.Where(f => f.FineDate.Date >= startDate.Value.Date).ToList();
        
        if (endDate.HasValue)
            fines = fines.Where(f => f.FineDate.Date <= endDate.Value.Date).ToList();

        var dtos = fines.Select(f => new FineReportDto(
            f.Id,
            members.FirstOrDefault(m => m.Id == f.MemberId) != null 
                ? $"{members.FirstOrDefault(m => m.Id == f.MemberId)?.FirstName} {members.FirstOrDefault(m => m.Id == f.MemberId)?.LastName}" 
                : "Unknown",
            f.Amount,
            f.PaidAmount,
            f.RemainingAmount,
            f.PaymentStatus,
            f.FineDate
        ));

        return ServiceResult<IEnumerable<FineReportDto>>.Succeeded(dtos);
    }
}