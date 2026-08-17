using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;

namespace LibraryManagementSystem.Application.Services;

public class SearchService : ISearchService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IAuthorRepository _authorRepository;

    public SearchService(
        IBookRepository bookRepository, 
        IMemberRepository memberRepository, 
        IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _authorRepository = authorRepository;
    }

    public async Task<ServiceResult<SearchResultDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return ServiceResult<SearchResultDto>.Succeeded(new SearchResultDto(
                new List<BookDto>(), new List<MemberDto>(), new List<AuthorDto>()));
        }

        var term = searchTerm.ToLower();

        // Search Books
        var books = await _bookRepository.GetAllAsync();
        var filteredBooks = books.Where(b => 
            b.Title.ToLower().Contains(term) || 
            b.ISBN.ToLower().Contains(term)).Take(10).ToList();

        var bookDtos = filteredBooks.Select(b => new BookDto(
            b.Id, b.ISBN, b.Title, b.Subtitle, 
            b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown", 
            b.Category != null ? b.Category.Name : "Unknown", 
            b.Publisher != null ? b.Publisher.Name : "Unknown", 
            b.TotalCopies, b.AvailableCopies, b.IsActive, b.CoverImageUrl
        ));

        // Search Members
        var members = await _memberRepository.GetAllAsync();
        var filteredMembers = members.Where(m => 
            m.FirstName.ToLower().Contains(term) || 
            m.LastName.ToLower().Contains(term) || 
            m.Email.ToLower().Contains(term) ||
            m.MembershipNumber.ToLower().Contains(term)).Take(10).ToList();

        var memberDtos = filteredMembers.Select(m => new MemberDto(
            m.Id, m.MembershipNumber, m.ApplicationUserId, m.FirstName, m.LastName, m.Email, 
            m.Phone, m.Address, m.DateOfBirth, m.MembershipType, m.MaxBooksAllowed, 
            m.MembershipDate, m.MembershipExpiryDate, m.IsActive
        ));

        // Search Authors
        var authors = await _authorRepository.GetAllAsync();
        var filteredAuthors = authors.Where(a => 
            a.FirstName.ToLower().Contains(term) || 
            a.LastName.ToLower().Contains(term)).Take(10).ToList();

        var authorDtos = filteredAuthors.Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName));

        var result = new SearchResultDto(bookDtos, memberDtos, authorDtos);
        return ServiceResult<SearchResultDto>.Succeeded(result);
    }
}