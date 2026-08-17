using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IBookCopyRepository : IGenericRepository<BookCopy> 
{
    Task<bool> BarcodeExistsAsync(string barcode, Guid? excludeId = null);
    Task<bool> AccessionNumberExistsAsync(string accessionNumber, Guid? excludeId = null);
     Task<BookCopy?> GetCopyByBarcodeAsync(string barcode);
}