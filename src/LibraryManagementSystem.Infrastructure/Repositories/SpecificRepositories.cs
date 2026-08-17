using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Book?> GetBookWithDetailsAsync(Guid id)
    {
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.BookCopies)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null)
    {
        return await _context.Books
            .AnyAsync(b => b.ISBN == isbn && (excludeBookId == null || b.Id != excludeBookId));
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Books
            .Include(b => b.Author)
            .Where(b => b.Title.ToLower().Contains(term) || 
                        b.ISBN.Contains(term) || 
                        (b.Author != null && (b.Author.FirstName + " " + b.Author.LastName).ToLower().Contains(term)))
            .ToListAsync();
    }
}

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> NameExistsAsync(string firstName, string lastName, Guid? excludeId = null)
    {
        return await _context.Authors
            .AnyAsync(a => a.FirstName == firstName && a.LastName == lastName && (excludeId == null || a.Id != excludeId));
    }
}

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId));
    }
}

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _context.Publishers
            .AnyAsync(p => p.Name == name && (excludeId == null || p.Id != excludeId));
    }
}

public class BookCopyRepository : GenericRepository<BookCopy>, IBookCopyRepository
{
    public BookCopyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> BarcodeExistsAsync(string barcode, Guid? excludeId = null)
    {
        return await _context.BookCopies
            .AnyAsync(c => c.Barcode == barcode && (excludeId == null || c.Id != excludeId));
    }

    public async Task<bool> AccessionNumberExistsAsync(string accessionNumber, Guid? excludeId = null)
    {
        return await _context.BookCopies
            .AnyAsync(c => c.AccessionNumber == accessionNumber && (excludeId == null || c.Id != excludeId));
    }

    // NEW METHOD IMPLEMENTATION
    public async Task<BookCopy?> GetCopyByBarcodeAsync(string barcode)
    {
        return await _context.BookCopies
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Barcode == barcode);
    }
}

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Member?> GetMemberWithDetailsAsync(Guid id)
    {
        return await _context.Members
            .Include(m => m.Loans)
            .Include(m => m.Reservations)
            .Include(m => m.Fines)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> MembershipNumberExistsAsync(string number, Guid? excludeId = null)
    {
        return await _context.Members
            .AnyAsync(m => m.MembershipNumber == number && (excludeId == null || m.Id != excludeId));
    }

    public async Task<Member?> GetMemberByUserIdAsync(string userId)
    {
        return await _context.Members
            .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);
    }
}

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Loan?> GetLoanWithDetailsAsync(Guid id)
    {
        return await _context.Loans
            .Include(l => l.BookCopy)!
                .ThenInclude(bc => bc!.Book)
            .Include(l => l.Member)
            .Include(l => l.Fines)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Loan>> GetAllLoansWithDetailsAsync()
    {
        return await _context.Loans
            .Include(l => l.BookCopy)!
                .ThenInclude(bc => bc!.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId)
    {
        return await _context.Loans
            .Where(l => l.MemberId == memberId && l.Status == LoanStatus.Issued)
            .ToListAsync();
    }

    public async Task<int> GetActiveLoanCountByMemberAsync(Guid memberId)
    {
        return await _context.Loans
            .CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Issued);
    }
}

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Reservation>> GetReservationsByBookAsync(Guid bookId)
    {
        return await _context.Reservations
            .Where(r => r.BookId == bookId)
            .OrderBy(r => r.QueuePosition)
            .ToListAsync();
    }
}

public class FineRepository : GenericRepository<Fine>, IFineRepository
{
    public FineRepository(ApplicationDbContext context) : base(context) { }

    public async Task<decimal> GetTotalUnpaidFinesByMemberAsync(Guid memberId)
    {
        return await _context.Fines
            .Where(f => f.MemberId == memberId && 
                        (f.PaymentStatus == FinePaymentStatus.Pending || f.PaymentStatus == FinePaymentStatus.PartiallyPaid))
            .SumAsync(f => f.RemainingAmount);
    }
}

public class FinePaymentRepository : GenericRepository<FinePayment>, IFinePaymentRepository
{
    public FinePaymentRepository(ApplicationDbContext context) : base(context) { }
}

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<int> GetUnreadCountByUserAsync(string userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context) { }
}

public class SettingsRepository : GenericRepository<SystemSetting>, ISettingsRepository
{
    public SettingsRepository(ApplicationDbContext context) : base(context) { }

    public async Task<SystemSetting?> GetSettingByKeyAsync(string key)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);
    }
}
