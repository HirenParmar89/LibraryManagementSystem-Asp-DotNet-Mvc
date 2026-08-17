using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface ISettingsRepository : IGenericRepository<SystemSetting> 
{
    Task<SystemSetting?> GetSettingByKeyAsync(string key);
}