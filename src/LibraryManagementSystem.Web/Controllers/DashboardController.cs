using LibraryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IEmailService _emailService;

    public DashboardController(IDashboardService dashboardService, IEmailService emailService)
    {
        _dashboardService = dashboardService;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _dashboardService.GetDashboardDataAsync();
        
        if (!result.Success || result.Data == null)
        {
            return View("Error");
        }

        return View(result.Data);
    }

    // GET: Dashboard/SendTestEmail
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> SendTestEmail()
    {
        try
        {
            // Sends an email to the currently logged in user's email address
            var userEmail = User.Identity?.Name; 
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Could not determine user email.";
                return RedirectToAction(nameof(Index));
            }

            var subject = "Library Management System - Test Email";
            var message = $"<h1>Welcome to the Library!</h1><p>This is a test email from your Library Management System.</p><p>Time: {DateTime.UtcNow}</p>";

            await _emailService.SendEmailAsync(userEmail, subject, message);
            
            TempData["SuccessMessage"] = $"Test email sent successfully to {userEmail}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to send email: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}