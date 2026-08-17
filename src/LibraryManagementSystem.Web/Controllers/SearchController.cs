using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

public class SearchController : Controller
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    // GET: Search?term=...
    public async Task<IActionResult> Index(string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return View(new SearchResultDto(new List<BookDto>(), new List<MemberDto>(), new List<AuthorDto>()));
        }

        var result = await _searchService.SearchAsync(term);
        
        ViewBag.SearchTerm = term;
        
        return View(result.Data ?? new SearchResultDto(new List<BookDto>(), new List<MemberDto>(), new List<AuthorDto>()));
    }
}