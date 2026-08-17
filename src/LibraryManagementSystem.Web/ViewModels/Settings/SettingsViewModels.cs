using LibraryManagementSystem.Application.DTOs;

namespace LibraryManagementSystem.Web.ViewModels.Settings;

public class SettingsViewModel
{
    public List<SystemSettingDto> Settings { get; set; } = new();
}