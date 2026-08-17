using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Services;

public interface IMemberService
{
    Task<ServiceResult<MemberDto>> GetMemberByIdAsync(Guid id);
    Task<ServiceResult<PagedResult<MemberDto>>> GetPagedMembersAsync(int page, int pageSize, string? searchTerm);
    Task<ServiceResult<Member>> CreateMemberAsync(Member member);
    Task<ServiceResult<Member>> UpdateMemberAsync(Member member);
    Task<ServiceResult> DeactivateMemberAsync(Guid id);
}