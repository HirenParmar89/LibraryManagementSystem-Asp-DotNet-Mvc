using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public class BookCopyService : IBookCopyService
{
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookCopyService(IBookCopyRepository bookCopyRepository, IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _bookCopyRepository = bookCopyRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<IEnumerable<BookCopyDto>>> GetAllCopiesAsync()
    {
        var copies = await _bookCopyRepository.GetAllAsync();
        var bookIds = copies.Select(c => c.BookId).Distinct().ToList();
        var books = (await _bookRepository.GetAllAsync()).Where(b => bookIds.Contains(b.Id)).ToList();

        var dtos = copies.Select(c => MapToDto(c, books.FirstOrDefault(b => b.Id == c.BookId)?.Title ?? "Unknown"));
        return ServiceResult<IEnumerable<BookCopyDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<IEnumerable<BookCopyDto>>> GetCopiesByBookAsync(Guid bookId)
    {
        var copies = await _bookCopyRepository.GetAllAsync();
        var filtered = copies.Where(c => c.BookId == bookId);
        
        var book = await _bookRepository.GetByIdAsync(bookId);
        var bookTitle = book?.Title ?? "Unknown";

        var dtos = filtered.Select(c => MapToDto(c, bookTitle));
        return ServiceResult<IEnumerable<BookCopyDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<BookCopyDto>> GetCopyByIdAsync(Guid id)
    {
        var copy = await _bookCopyRepository.GetByIdAsync(id);
        if (copy == null) return ServiceResult<BookCopyDto>.Failed("Book copy not found.");

        var book = await _bookRepository.GetByIdAsync(copy.BookId);
        return ServiceResult<BookCopyDto>.Succeeded(MapToDto(copy, book?.Title ?? "Unknown"));
    }

    public async Task<ServiceResult<BookCopyDto>> GetCopyByBarcodeAsync(string barcode)
    {
        var copy = await _bookCopyRepository.GetCopyByBarcodeAsync(barcode);
        if (copy == null) return ServiceResult<BookCopyDto>.Failed("Book copy not found.");

        var book = await _bookRepository.GetByIdAsync(copy.BookId);
        return ServiceResult<BookCopyDto>.Succeeded(MapToDto(copy, book?.Title ?? "Unknown"));
    }

    public async Task<ServiceResult<BookCopy>> CreateCopyAsync(BookCopy copy)
    {
        if (await _bookCopyRepository.BarcodeExistsAsync(copy.Barcode))
            return ServiceResult<BookCopy>.Failed("Barcode already exists.");

        if (await _bookCopyRepository.AccessionNumberExistsAsync(copy.AccessionNumber))
            return ServiceResult<BookCopy>.Failed("Accession Number already exists.");

        var book = await _bookRepository.GetByIdAsync(copy.BookId);
        if (book != null)
        {
            book.TotalCopies++;
            if (copy.IsAvailable) book.AvailableCopies++;
            _bookRepository.Update(book);
        }

        await _bookCopyRepository.AddAsync(copy);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<BookCopy>.Succeeded(copy);
    }

    public async Task<ServiceResult<BookCopy>> UpdateCopyAsync(BookCopy copy)
    {
        if (await _bookCopyRepository.BarcodeExistsAsync(copy.Barcode, copy.Id))
            return ServiceResult<BookCopy>.Failed("Barcode already exists.");

        if (await _bookCopyRepository.AccessionNumberExistsAsync(copy.AccessionNumber, copy.Id))
            return ServiceResult<BookCopy>.Failed("Accession Number already exists.");

        _bookCopyRepository.Update(copy);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<BookCopy>.Succeeded(copy);
    }

    public async Task<ServiceResult> DeleteCopyAsync(Guid id)
    {
        var copy = await _bookCopyRepository.GetByIdAsync(id);
        if (copy == null) return ServiceResult.Failed("Book copy not found.");

        if (copy.Status == BookCopyStatus.Issued)
            return ServiceResult.Failed("Cannot delete a copy that is currently issued.");

        var book = await _bookRepository.GetByIdAsync(copy.BookId);
        if (book != null)
        {
            book.TotalCopies--;
            if (copy.IsAvailable) book.AvailableCopies--;
            _bookRepository.Update(book);
        }

        _bookCopyRepository.Delete(copy);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    private static BookCopyDto MapToDto(BookCopy copy, string bookTitle)
    {
        return new BookCopyDto(
            copy.Id,
            copy.BookId,
            bookTitle,
            copy.AccessionNumber,
            copy.Barcode,
            copy.Condition,
            copy.Status,
            copy.IsAvailable
        );
    }
}