using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Application.Options;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using Microsoft.Extensions.Options;

namespace LibraryManagementSystem.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IFineRepository _fineRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineService _fineService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LibrarySettings _librarySettings;

    public LoanService(
        ILoanRepository loanRepository,
        IBookCopyRepository bookCopyRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IFineRepository fineRepository,
        IReservationRepository reservationRepository,
        IFineService fineService,
        IUnitOfWork unitOfWork,
        IOptions<LibrarySettings> librarySettings)
    {
        _loanRepository = loanRepository;
        _bookCopyRepository = bookCopyRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _fineRepository = fineRepository;
        _reservationRepository = reservationRepository;
        _fineService = fineService;
        _unitOfWork = unitOfWork;
        _librarySettings = librarySettings.Value;
    }

    public async Task<ServiceResult<LoanDto>> IssueBookAsync(IssueBookDto dto)
    {
        var member = await _memberRepository.GetByIdAsync(dto.MemberId);
        if (member == null || !member.IsActive)
            return ServiceResult<LoanDto>.Failed("Member not found or inactive.");

        if (member.MembershipExpiryDate <= DateTime.UtcNow)
            return ServiceResult<LoanDto>.Failed("Membership has expired. Please renew membership.");

        var activeLoansCount = await _loanRepository.GetActiveLoanCountByMemberAsync(member.Id);
        if (activeLoansCount >= member.MaxBooksAllowed)
            return ServiceResult<LoanDto>.Failed($"Member has reached the maximum borrowing limit of {member.MaxBooksAllowed} books.");

        if (_librarySettings.BlockIssueOnFine)
        {
            var unpaidFines = await _fineRepository.GetTotalUnpaidFinesByMemberAsync(member.Id);
            if (unpaidFines > 0)
                return ServiceResult<LoanDto>.Failed($"Member has outstanding fines of {unpaidFines:C}. Please clear dues before issuing new books.");
        }

        var copy = await _bookCopyRepository.GetByIdAsync(dto.BookCopyId);
        if (copy == null || copy.Status != BookCopyStatus.Available)
            return ServiceResult<LoanDto>.Failed("Book copy is not available for issue.");

        var loan = new Loan
        {
            BookCopyId = copy.Id,
            MemberId = member.Id,
            IssuedByUserId = dto.IssuedByUserId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(_librarySettings.DefaultLoanDurationDays),
            Status = LoanStatus.Issued
        };

        await _loanRepository.AddAsync(loan);

        copy.Status = BookCopyStatus.Issued;
        copy.IsAvailable = false;
        _bookCopyRepository.Update(copy);

        var book = await _bookRepository.GetByIdAsync(copy.BookId);
        if (book != null)
        {
            book.AvailableCopies--;
            _bookRepository.Update(book);
        }

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<LoanDto>.Succeeded(MapToDto(loan, book, copy, member));
    }

    public async Task<ServiceResult<LoanDto>> ReturnBookAsync(ReturnBookDto dto)
    {
        var loan = await _loanRepository.GetLoanWithDetailsAsync(dto.LoanId);
        if (loan == null || loan.Status != LoanStatus.Issued)
            return ServiceResult<LoanDto>.Failed("Active loan not found.");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;
        loan.UpdatedAt = DateTime.UtcNow;

        if (loan.DueDate < loan.ReturnDate)
        {
            var overdueDays = (loan.ReturnDate.Value - loan.DueDate).Days - _librarySettings.FineGracePeriodDays;
            if (overdueDays > 0)
            {
                var fineAmount = overdueDays * _librarySettings.DailyFineAmount;
                loan.FineAmount = fineAmount;
                await _fineService.GenerateFineForOverdueLoanAsync(loan.Id);
            }
        }

        var copy = loan.BookCopy;
        if (copy != null)
        {
            copy.Status = BookCopyStatus.Available;
            copy.IsAvailable = true;
            _bookCopyRepository.Update(copy);

            var book = await _bookRepository.GetByIdAsync(copy.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
                _bookRepository.Update(book);
            }
        }

        _loanRepository.Update(loan);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<LoanDto>.Succeeded(MapToDto(loan, loan.BookCopy?.Book, copy, loan.Member));
    }

    public async Task<ServiceResult<LoanDto>> RenewBookAsync(RenewBookDto dto)
    {
        var loan = await _loanRepository.GetLoanWithDetailsAsync(dto.LoanId);
        if (loan == null || loan.Status != LoanStatus.Issued)
            return ServiceResult<LoanDto>.Failed("Active loan not found.");

        if (loan.RenewalCount >= _librarySettings.MaxRenewals)
            return ServiceResult<LoanDto>.Failed($"Maximum renewal limit of {_librarySettings.MaxRenewals} reached.");

        var bookId = loan.BookCopy?.BookId ?? Guid.Empty;
        if (bookId != Guid.Empty)
        {
            var reservations = await _reservationRepository.GetReservationsByBookAsync(bookId);
            if (reservations.Any(r => r.Status == ReservationStatus.Pending))
                return ServiceResult<LoanDto>.Failed("Cannot renew. Book has pending reservations from other members.");
        }

        loan.DueDate = loan.DueDate.AddDays(_librarySettings.DefaultLoanDurationDays);
        loan.RenewalCount++;
        loan.UpdatedAt = DateTime.UtcNow;

        _loanRepository.Update(loan);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<LoanDto>.Succeeded(MapToDto(loan, loan.BookCopy?.Book, loan.BookCopy, loan.Member));
    }

    public async Task<ServiceResult<IEnumerable<LoanDto>>> GetActiveLoansAsync()
    {
        var loans = await _loanRepository.GetAllLoansWithDetailsAsync();
        var active = loans.Where(l => l.Status == LoanStatus.Issued).ToList();
        return ServiceResult<IEnumerable<LoanDto>>.Succeeded(active.Select(l => MapToDto(l, l.BookCopy?.Book, l.BookCopy, l.Member)));
    }

    public async Task<ServiceResult<IEnumerable<LoanDto>>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetAllLoansWithDetailsAsync();
        var overdue = loans.Where(l => l.Status == LoanStatus.Issued && l.DueDate < DateTime.UtcNow).ToList();
        return ServiceResult<IEnumerable<LoanDto>>.Succeeded(overdue.Select(l => MapToDto(l, l.BookCopy?.Book, l.BookCopy, l.Member)));
    }

    public async Task<ServiceResult<IEnumerable<LoanDto>>> GetLoanHistoryAsync()
    {
        var loans = await _loanRepository.GetAllLoansWithDetailsAsync();
        return ServiceResult<IEnumerable<LoanDto>>.Succeeded(loans.Select(l => MapToDto(l, l.BookCopy?.Book, l.BookCopy, l.Member)));
    }

    private static LoanDto MapToDto(Loan loan, Book? book, BookCopy? copy, Member? member)
    {
        return new LoanDto(
            loan.Id,
            book?.Title ?? "Unknown",
            book?.ISBN ?? "Unknown",
            copy?.Barcode ?? "Unknown",
            member != null ? $"{member.FirstName} {member.LastName}" : "Unknown",
            loan.IssueDate,
            loan.DueDate,
            loan.ReturnDate,
            loan.Status,
            loan.FineAmount
        );
    }
}