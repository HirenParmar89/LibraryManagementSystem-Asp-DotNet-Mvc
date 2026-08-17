using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation> 
{
    Task<IEnumerable<Reservation>> GetReservationsByBookAsync(Guid bookId);
}