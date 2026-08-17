using LibraryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraryManagementSystem.Web.Filters;

public class AuditActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        // Only audit POST, PUT, DELETE requests (write operations)
        var method = context.HttpContext.Request.Method;
        if (method != "POST" && method != "PUT" && method != "DELETE")
        {
            return;
        }

        // Skip if it's an API call or area we don't want to audit (like Account/Login failures)
        if (executedContext.Exception != null) return;

        var userId = context.HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var actionName = context.ActionDescriptor.RouteValues["action"] ?? "Unknown";
        var controllerName = context.ActionDescriptor.RouteValues["controller"] ?? "Unknown";
        
        // Try to get the ID from route values or action arguments
        string? entityId = null;
        if (context.RouteData.Values.TryGetValue("id", out var idObj))
        {
            entityId = idObj?.ToString();
        }
        else
        {
            // Try to find a model with an Id property
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;
                var idProp = arg.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    entityId = idProp.GetValue(arg)?.ToString();
                    break;
                }
            }
        }

        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        var auditService = context.HttpContext.RequestServices.GetRequiredService<IAuditService>();
        
        await auditService.LogActionAsync(
            userId, 
            actionName, 
            controllerName, 
            entityId, 
            null, // OldValues - capturing this requires EF Core interceptors which is out of scope for this filter
            null, // NewValues
            ipAddress
        );
    }
}