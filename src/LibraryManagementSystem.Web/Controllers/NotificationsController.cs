using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Web.ViewModels.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

    public NotificationsController(INotificationService notificationService, UserManager<Infrastructure.Identity.ApplicationUser> userManager)
    {
        _notificationService = notificationService;
        _userManager = userManager;
    }

    // GET: Notifications
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var notifResult = await _notificationService.GetNotificationsForUserAsync(user.Id);
        var countResult = await _notificationService.GetUnreadNotificationCountAsync(user.Id);

        var viewModel = new NotificationListViewModel
        {
            Notifications = notifResult.Data ?? new List<NotificationDto>(),
            UnreadCount = countResult.Data
        };

        return View(viewModel);
    }

    // POST: Notifications/MarkAsRead/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id, string? returnUrl = null)
    {
        await _notificationService.MarkAsReadAsync(id);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Notifications/MarkAllAsRead
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            await _notificationService.MarkAllAsReadAsync(user.Id);
        }
        return RedirectToAction(nameof(Index));
    }
}