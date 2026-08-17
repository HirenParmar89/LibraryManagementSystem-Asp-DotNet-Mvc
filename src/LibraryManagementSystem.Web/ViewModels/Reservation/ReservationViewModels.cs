using LibraryManagementSystem.Application.DTOs;

namespace LibraryManagementSystem.Web.ViewModels.Reservation;

public class ReservationListViewModel
{
    public IEnumerable<ReservationDto> Reservations { get; set; } = new List<ReservationDto>();
}

public class CreateReservationViewModel
{
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    
    // For dropdowns
    public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>();
    public IEnumerable<MemberDto> Members { get; set; } = new List<MemberDto>();
}