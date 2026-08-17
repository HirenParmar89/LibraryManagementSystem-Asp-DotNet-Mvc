using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> SendNotificationAsync(string userId, string title, string message, NotificationType type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult<int>> GetUnreadNotificationCountAsync(string userId)
    {
        var count = await _notificationRepository.GetUnreadCountByUserAsync(userId);
        return ServiceResult<int>.Succeeded(count);
    }

    public async Task<ServiceResult<IEnumerable<NotificationDto>>> GetNotificationsForUserAsync(string userId)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserAsync(userId);
        var dtos = notifications.Select(n => new NotificationDto(
            n.Id,
            n.Title,
            n.Message,
            n.Type,
            n.IsRead,
            n.CreatedAt
        ));

        return ServiceResult<IEnumerable<NotificationDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult> MarkAsReadAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null) return ServiceResult.Failed("Notification not found.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult> MarkAllAsReadAsync(string userId)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserAsync(userId);
        var unread = notifications.Where(n => !n.IsRead).ToList();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(notification);
        }

        if (unread.Any())
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return ServiceResult.Succeeded();
    }
}