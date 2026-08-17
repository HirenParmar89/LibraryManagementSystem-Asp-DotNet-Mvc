using LibraryManagementSystem.Application.DTOs;

namespace LibraryManagementSystem.Web.ViewModels.Notification;

public class NotificationListViewModel
{
    public IEnumerable<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
    public int UnreadCount { get; set; }
}