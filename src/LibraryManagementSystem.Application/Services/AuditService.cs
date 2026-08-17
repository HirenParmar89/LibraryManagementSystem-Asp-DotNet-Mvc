using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> LogActionAsync(string? userId, string action, string entityName, string? entityId, string? oldValues, string? newValues, string? ipAddress)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    public async Task<ServiceResult<IEnumerable<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 50)
    {
        var logs = await _auditLogRepository.GetAllAsync();
        
        // In a real app, this pagination should be done in the repository via IQueryable
        var pagedLogs = logs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = pagedLogs.Select(l => new AuditLogDto(
            l.Id,
            l.UserId,
            l.UserId, // Note: To get the actual UserName, we'd need to join with Identity. Passing UserId for now.
            l.Action,
            l.EntityName,
            l.EntityId,
            l.Timestamp,
            l.IpAddress
        ));

        return ServiceResult<IEnumerable<AuditLogDto>>.Succeeded(dtos);
    }
}