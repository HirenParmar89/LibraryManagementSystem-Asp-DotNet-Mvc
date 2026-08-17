using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.ViewModels;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Interfaces.Services;

public interface IAuthorService
{
    Task<ServiceResult<IEnumerable<AuthorDto>>> GetAllAuthorsAsync();
    Task<ServiceResult<AuthorDto>> GetAuthorByIdAsync(Guid id);
    Task<ServiceResult<Author>> CreateAuthorAsync(Author author);
    Task<ServiceResult<Author>> UpdateAuthorAsync(Author author);
    Task<ServiceResult> DeleteAuthorAsync(Guid id);
}

public interface ICategoryService
{
    Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid id);
    Task<ServiceResult<Category>> CreateCategoryAsync(Category category);
    Task<ServiceResult<Category>> UpdateCategoryAsync(Category category);
    Task<ServiceResult> DeleteCategoryAsync(Guid id);
}

public interface IPublisherService
{
    Task<ServiceResult<IEnumerable<PublisherDto>>> GetAllPublishersAsync();
    Task<ServiceResult<PublisherDto>> GetPublisherByIdAsync(Guid id);
    Task<ServiceResult<Publisher>> CreatePublisherAsync(Publisher publisher);
    Task<ServiceResult<Publisher>> UpdatePublisherAsync(Publisher publisher);
    Task<ServiceResult> DeletePublisherAsync(Guid id);
}

public interface IReservationService
{
    Task<ServiceResult> CreateReservationAsync(Guid bookId, Guid memberId);
    Task<ServiceResult> CancelReservationAsync(Guid reservationId);
    Task<ServiceResult> FulfillReservationAsync(Guid reservationId);
    Task<ServiceResult<IEnumerable<ReservationDto>>> GetAllReservationsAsync();
}

public interface IFineService
{
    Task<ServiceResult> GenerateFineForOverdueLoanAsync(Guid loanId);
    Task<ServiceResult> WaiveFineAsync(Guid fineId);
    Task<ServiceResult<IEnumerable<FineDto>>> GetAllFinesAsync();
    Task<ServiceResult<FineDto>> GetFineByIdAsync(Guid id);
    Task<ServiceResult> RecordPaymentAsync(FinePaymentDto dto);
}

public interface IFinePaymentService
{
    Task<ServiceResult> RecordPaymentAsync(Guid fineId, decimal amount, PaymentMethod method, string receivedByUserId);
}

public interface INotificationService
{
    Task<ServiceResult> SendNotificationAsync(string userId, string title, string message, NotificationType type);
    Task<ServiceResult<int>> GetUnreadNotificationCountAsync(string userId);
    Task<ServiceResult<IEnumerable<NotificationDto>>> GetNotificationsForUserAsync(string userId);
    Task<ServiceResult> MarkAsReadAsync(Guid id);
    Task<ServiceResult> MarkAllAsReadAsync(string userId);
}

public interface IAuditService
{
    Task<ServiceResult> LogActionAsync(string? userId, string action, string entityName, string? entityId, string? oldValues, string? newValues, string? ipAddress);
    Task<ServiceResult<IEnumerable<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 50);
}

public interface IReportService
{
    Task<ServiceResult<IEnumerable<CirculationReportDto>>> GetCirculationReportAsync(DateTime? startDate, DateTime? endDate);
    Task<ServiceResult<IEnumerable<InventoryReportDto>>> GetInventoryReportAsync();
    Task<ServiceResult<IEnumerable<MemberReportDto>>> GetMemberReportAsync(DateTime? startDate, DateTime? endDate);
    Task<ServiceResult<IEnumerable<FineReportDto>>> GetFineReportAsync(DateTime? startDate, DateTime? endDate);
}

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}

public interface ISettingsService
{
    Task<ServiceResult<IEnumerable<SystemSettingDto>>> GetAllSettingsAsync();
    Task<ServiceResult> UpdateSettingsAsync(Dictionary<string, string> settings);
}

public interface IDashboardService
{
    Task<ServiceResult<DashboardViewModel>> GetDashboardDataAsync();
}

public interface ISearchService
{
    Task<ServiceResult<SearchResultDto>> SearchAsync(string searchTerm);
}

public interface IBookCopyService
{
    Task<ServiceResult<IEnumerable<BookCopyDto>>> GetAllCopiesAsync();
    Task<ServiceResult<IEnumerable<BookCopyDto>>> GetCopiesByBookAsync(Guid bookId);
    Task<ServiceResult<BookCopyDto>> GetCopyByIdAsync(Guid id);
    Task<ServiceResult<BookCopy>> CreateCopyAsync(BookCopy copy);
    Task<ServiceResult<BookCopy>> UpdateCopyAsync(BookCopy copy);
    Task<ServiceResult> DeleteCopyAsync(Guid id);
    Task<ServiceResult<BookCopyDto>> GetCopyByBarcodeAsync(string barcode);
}
