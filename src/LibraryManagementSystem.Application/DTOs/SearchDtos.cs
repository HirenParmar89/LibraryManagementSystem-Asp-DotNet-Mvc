namespace LibraryManagementSystem.Application.DTOs;

public record SearchResultDto(
    IEnumerable<BookDto> Books,
    IEnumerable<MemberDto> Members,
    IEnumerable<AuthorDto> Authors
);