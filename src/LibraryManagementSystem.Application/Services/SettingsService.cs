using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SettingsService(ISettingsRepository settingsRepository, IUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<IEnumerable<SystemSettingDto>>> GetAllSettingsAsync()
    {
        var settings = await _settingsRepository.GetAllAsync();
        var dtos = settings.Select(s => new SystemSettingDto(s.Id, s.Key, s.Value, s.Description));
        
        return ServiceResult<IEnumerable<SystemSettingDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult> UpdateSettingsAsync(Dictionary<string, string> settings)
    {
        var existingSettings = await _settingsRepository.GetAllAsync();
        
        foreach (var setting in existingSettings)
        {
            if (settings.TryGetValue(setting.Key, out var newValue))
            {
                if (setting.Value != newValue)
                {
                    setting.Value = newValue;
                    setting.UpdatedAt = DateTime.UtcNow;
                    _settingsRepository.Update(setting);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Succeeded();
    }
}