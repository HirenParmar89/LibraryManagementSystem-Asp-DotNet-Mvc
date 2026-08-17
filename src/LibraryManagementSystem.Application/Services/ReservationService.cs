using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(
        IReservationRepository reservationRepository, 
        IBookRepository bookRepository, 
        IMemberRepository memberRepository, 
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> CreateReservationAsync(Guid bookId, Guid memberId)
    {
        var book = await _bookRepository.GetBookWithDetailsAsync(bookId);
        if (book == null) return ServiceResult.Failed("Book not found.");

        // Only allow reservation if no copies are available
        if (book.AvailableCopies > 0)
            return ServiceResult.Failed("Book is currently available. No need to reserve. Please issue it directly.");

        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null || !member.IsActive) return ServiceResult.Failed("Member not found or inactive.");

        // Check if member already has a pending reservation for this book
        var existingReservations = await _reservationRepository.GetReservationsByBookAsync(bookId);
        if (existingReservations.Any(r => r.MemberId == memberId && r.Status == ReservationStatus.Pending))
            return ServiceResult.Failed("You already have a pending reservation for this book.");

        // Calculate Queue Position
        var nextPosition = existingReservations.Count(r => r.Status == ReservationStatus.Pending) + 1;

        var reservation = new Reservation
        {
            BookId = bookId,
            MemberId = memberId,
            ReservationDate = DateTime.UtcNow,
            Status = ReservationStatus.Pending,
            QueuePosition = nextPosition
        };

        await _reservationRepository.AddAsync(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult> CancelReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null) return ServiceResult.Failed("Reservation not found.");

        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Ready)
            return ServiceResult.Failed("Cannot cancel a completed or expired reservation.");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        
        // Re-calculate queue positions for remaining pending reservations
        await RecalculateQueuePositions(reservation.BookId);
        
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult> FulfillReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null) return ServiceResult.Failed("Reservation not found.");

        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Ready)
            return ServiceResult.Failed("Cannot fulfill a completed or cancelled reservation.");

        reservation.Status = ReservationStatus.Completed;
        reservation.UpdatedAt = DateTime.UtcNow;
        _reservationRepository.Update(reservation);
        
        await RecalculateQueuePositions(reservation.BookId);
        
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult<IEnumerable<ReservationDto>>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        var bookIds = reservations.Select(r => r.BookId).Distinct().ToList();
        var memberIds = reservations.Select(r => r.MemberId).Distinct().ToList();

        var books = (await _bookRepository.GetAllAsync()).Where(b => bookIds.Contains(b.Id)).ToList();
        var members = (await _memberRepository.GetAllAsync()).Where(m => memberIds.Contains(m.Id)).ToList();

        var dtos = reservations.Select(r => new ReservationDto(
            r.Id,
            r.BookId,
            books.FirstOrDefault(b => b.Id == r.BookId)?.Title ?? "Unknown",
            r.MemberId,
            members.FirstOrDefault(m => m.Id == r.MemberId) != null 
                ? $"{members.FirstOrDefault(m => m.Id == r.MemberId)?.FirstName} {members.FirstOrDefault(m => m.Id == r.MemberId)?.LastName}" 
                : "Unknown",
            r.ReservationDate,
            r.ExpiryDate,
            r.Status,
            r.QueuePosition
        ));

        return ServiceResult<IEnumerable<ReservationDto>>.Succeeded(dtos);
    }

    private async Task RecalculateQueuePositions(Guid bookId)
    {
        var pendingReservations = (await _reservationRepository.GetReservationsByBookAsync(bookId))
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.ReservationDate)
            .ToList();

        for (int i = 0; i < pendingReservations.Count; i++)
        {
            pendingReservations[i].QueuePosition = i + 1;
            _reservationRepository.Update(pendingReservations[i]);
        }
    }
}