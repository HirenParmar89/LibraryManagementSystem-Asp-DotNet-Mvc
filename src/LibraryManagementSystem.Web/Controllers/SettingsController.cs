using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Application.Options;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Web.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly LibrarySettings _librarySettings;

    public SettingsController(ISettingsService settingsService, IOptions<LibrarySettings> librarySettings)
    {
        _settingsService = settingsService;
        _librarySettings = librarySettings.Value;
    }

    // GET: Settings
    public async Task<IActionResult> Index()
    {
        var result = await _settingsService.GetAllSettingsAsync();
        var settings = result.Data?.ToList() ?? new List<SystemSettingDto>();

        // Ensure default settings exist if the table is empty
        if (!settings.Any())
        {
            // In a real app, you'd seed this. For now, we just show a message or handle it gracefully.
            // Let's create a dummy list for the UI if empty, so the form doesn't break.
            settings = new List<SystemSettingDto>
            {
                new(Guid.NewGuid(), "LibraryName", _librarySettings.LibraryName, "Name of the Library"),
                new(Guid.NewGuid(), "DefaultLoanDurationDays", _librarySettings.DefaultLoanDurationDays.ToString(), "Default days a book can be loaned"),
                new(Guid.NewGuid(), "MaxBooksPerMember", _librarySettings.MaxBooksPerMember.ToString(), "Maximum books a member can borrow"),
                new(Guid.NewGuid(), "DailyFineAmount", _librarySettings.DailyFineAmount.ToString(), "Fine amount per day for overdue books"),
                new(Guid.NewGuid(), "FineGracePeriodDays", _librarySettings.FineGracePeriodDays.ToString(), "Grace period before fine starts"),
                new(Guid.NewGuid(), "MaxRenewals", _librarySettings.MaxRenewals.ToString(), "Maximum renewals allowed per loan")
            };
        }

        var viewModel = new SettingsViewModel
        {
            Settings = settings
        };

        return View(viewModel);
    }

    // POST: Settings
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var settingsDict = new Dictionary<string, string>();
        foreach (var item in model.Settings)
        {
            settingsDict[item.Key] = item.Value ?? string.Empty;
        }

        var result = await _settingsService.UpdateSettingsAsync(settingsDict);
        
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Failed to update settings.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Settings updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}