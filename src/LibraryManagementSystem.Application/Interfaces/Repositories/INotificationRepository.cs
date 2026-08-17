using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface INotificationRepository : IGenericRepository<Notification> 
{
    Task<int> GetUnreadCountByUserAsync(string userId);
    Task<IEnumerable<Notification>> GetNotificationsByUserAsync(string userId);
}