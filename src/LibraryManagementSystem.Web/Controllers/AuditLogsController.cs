using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AuditLogsController : Controller
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    // GET: AuditLogs
    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var result = await _auditService.GetAuditLogsAsync(page, pageSize);
        var logs = result.Data ?? new List<AuditLogDto>();
        
        ViewBag.PageNumber = page;
        ViewBag.PageSize = pageSize;
        
        return View(logs);
    }
}