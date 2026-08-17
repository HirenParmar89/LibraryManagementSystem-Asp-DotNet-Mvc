using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IAuthorRepository authorRepository, IUnitOfWork unitOfWork)
    {
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<IEnumerable<AuthorDto>>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAllAsync();
        var dtos = authors.Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName));
        return ServiceResult<IEnumerable<AuthorDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<AuthorDto>> GetAuthorByIdAsync(Guid id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null) return ServiceResult<AuthorDto>.Failed("Author not found.");

        return ServiceResult<AuthorDto>.Succeeded(new AuthorDto(author.Id, author.FirstName, author.LastName));
    }

    public async Task<ServiceResult<Author>> CreateAuthorAsync(Author author)
    {
        if (await _authorRepository.NameExistsAsync(author.FirstName, author.LastName))
            return ServiceResult<Author>.Failed("Author already exists.");

        await _authorRepository.AddAsync(author);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<Author>.Succeeded(author);
    }

    public async Task<ServiceResult<Author>> UpdateAuthorAsync(Author author)
    {
        if (await _authorRepository.NameExistsAsync(author.FirstName, author.LastName, author.Id))
            return ServiceResult<Author>.Failed("Author already exists.");

        author.UpdatedAt = DateTime.UtcNow;
        _authorRepository.Update(author);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<Author>.Succeeded(author);
    }

    public async Task<ServiceResult> DeleteAuthorAsync(Guid id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null) return ServiceResult.Failed("Author not found.");

        // Soft delete
        author.IsActive = false;
        author.UpdatedAt = DateTime.UtcNow;
        _authorRepository.Update(author);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }
}