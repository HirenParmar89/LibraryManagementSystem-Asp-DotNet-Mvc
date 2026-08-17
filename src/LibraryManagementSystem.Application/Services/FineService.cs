using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public class FineService : IFineService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IFineRepository _fineRepository;
    private readonly IFinePaymentRepository _finePaymentRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FineService(
        ILoanRepository loanRepository, 
        IFineRepository fineRepository, 
        IFinePaymentRepository finePaymentRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _fineRepository = fineRepository;
        _finePaymentRepository = finePaymentRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> GenerateFineForOverdueLoanAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetLoanWithDetailsAsync(loanId);
        if (loan == null) return ServiceResult.Failed("Loan not found.");

        if (loan.FineAmount <= 0) return ServiceResult.Succeeded();

        var existingFines = await _fineRepository.GetAllAsync();
        if (existingFines.Any(f => f.LoanId == loanId))
            return ServiceResult.Succeeded();

        var lateDays = loan.ReturnDate.HasValue ? (loan.ReturnDate.Value - loan.DueDate).Days : 0;

        var fine = new Fine
        {
            LoanId = loanId,
            MemberId = loan.MemberId,
            Amount = loan.FineAmount,
            Reason = $"Overdue return. Book returned {lateDays} days late.",
            FineDate = DateTime.UtcNow,
            RemainingAmount = loan.FineAmount,
            PaymentStatus = FinePaymentStatus.Pending
        };

        await _fineRepository.AddAsync(fine);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult> WaiveFineAsync(Guid fineId)
    {
        var fine = await _fineRepository.GetByIdAsync(fineId);
        if (fine == null) return ServiceResult.Failed("Fine not found.");

        if (fine.PaymentStatus == FinePaymentStatus.Paid)
            return ServiceResult.Failed("Cannot waive a fully paid fine.");

        fine.PaymentStatus = FinePaymentStatus.Waived;
        fine.RemainingAmount = 0;
        fine.PaidDate = DateTime.UtcNow;
        fine.Notes = "Fine waived by administrator.";

        _fineRepository.Update(fine);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult<IEnumerable<FineDto>>> GetAllFinesAsync()
    {
        var fines = await _fineRepository.GetAllAsync();
        var memberIds = fines.Select(f => f.MemberId).Distinct().ToList();
        var members = (await _memberRepository.GetAllAsync()).Where(m => memberIds.Contains(m.Id)).ToList();
        
        var dtos = fines.Select(f => new FineDto(
            f.Id,
            f.LoanId,
            f.MemberId,
            members.FirstOrDefault(m => m.Id == f.MemberId) != null 
                ? $"{members.FirstOrDefault(m => m.Id == f.MemberId)?.FirstName} {members.FirstOrDefault(m => m.Id == f.MemberId)?.LastName}" 
                : "Unknown",
            f.Amount,
            f.PaidAmount,
            f.RemainingAmount,
            f.PaymentStatus,
            f.FineDate
        ));

        return ServiceResult<IEnumerable<FineDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<FineDto>> GetFineByIdAsync(Guid id)
    {
        var fine = await _fineRepository.GetByIdAsync(id);
        if (fine == null) return ServiceResult<FineDto>.Failed("Fine not found.");

        var member = await _memberRepository.GetByIdAsync(fine.MemberId);

        var dto = new FineDto(
            fine.Id,
            fine.LoanId,
            fine.MemberId,
            member != null ? $"{member.FirstName} {member.LastName}" : "Unknown",
            fine.Amount,
            fine.PaidAmount,
            fine.RemainingAmount,
            fine.PaymentStatus,
            fine.FineDate
        );

        return ServiceResult<FineDto>.Succeeded(dto);
    }

    public async Task<ServiceResult> RecordPaymentAsync(FinePaymentDto dto)
    {
        var fine = await _fineRepository.GetByIdAsync(dto.FineId);
        if (fine == null) return ServiceResult.Failed("Fine not found.");

        if (fine.PaymentStatus == FinePaymentStatus.Paid || fine.PaymentStatus == FinePaymentStatus.Waived)
            return ServiceResult.Failed("Fine is already paid or waived.");

        if (dto.Amount <= 0)
            return ServiceResult.Failed("Payment amount must be greater than zero.");

        if (dto.Amount > fine.RemainingAmount)
            return ServiceResult.Failed($"Payment amount exceeds remaining balance of {fine.RemainingAmount:C}.");

        var payment = new FinePayment
        {
            FineId = fine.Id,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = dto.Method,
            ReceivedByUserId = dto.ReceivedByUserId
        };

        await _finePaymentRepository.AddAsync(payment);

        fine.PaidAmount += dto.Amount;
        fine.RemainingAmount -= dto.Amount;
        
        if (fine.RemainingAmount == 0)
        {
            fine.PaymentStatus = FinePaymentStatus.Paid;
            fine.PaidDate = DateTime.UtcNow;
        }
        else
        {
            fine.PaymentStatus = FinePaymentStatus.PartiallyPaid;
        }

        _fineRepository.Update(fine);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }
}