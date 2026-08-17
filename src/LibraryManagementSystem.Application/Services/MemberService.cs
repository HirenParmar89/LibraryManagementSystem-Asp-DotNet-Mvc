using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<MemberDto>> GetMemberByIdAsync(Guid id)
    {
        var member = await _memberRepository.GetMemberWithDetailsAsync(id);
        if (member == null) return ServiceResult<MemberDto>.Failed("Member not found.");

        return ServiceResult<MemberDto>.Succeeded(MapToDto(member));
    }

    public async Task<ServiceResult<PagedResult<MemberDto>>> GetPagedMembersAsync(int page, int pageSize, string? searchTerm)
    {
        var members = await _memberRepository.GetAllAsync();
        var query = members.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(m => 
                m.FirstName.ToLower().Contains(searchTerm) || 
                m.LastName.ToLower().Contains(searchTerm) || 
                m.MembershipNumber.Contains(searchTerm) ||
                m.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = query.Count();
        var pagedMembers = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        var pagedResult = new PagedResult<MemberDto>
        {
            Items = pagedMembers.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };

        return ServiceResult<PagedResult<MemberDto>>.Succeeded(pagedResult);
    }

    public async Task<ServiceResult<Member>> CreateMemberAsync(Member member)
    {
        if (await _memberRepository.MembershipNumberExistsAsync(member.MembershipNumber))
            return ServiceResult<Member>.Failed("Membership number already exists.");

        await _memberRepository.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<Member>.Succeeded(member);
    }

    public async Task<ServiceResult<Member>> UpdateMemberAsync(Member member)
    {
        member.UpdatedAt = DateTime.UtcNow;
        _memberRepository.Update(member);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<Member>.Succeeded(member);
    }

    public async Task<ServiceResult> DeactivateMemberAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) return ServiceResult.Failed("Member not found.");

        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;
        _memberRepository.Update(member);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto(
            member.Id,
            member.MembershipNumber,
            member.ApplicationUserId,
            member.FirstName,
            member.LastName,
            member.Email,
            member.Phone,
            member.Address,
            member.DateOfBirth,
            member.MembershipType,
            member.MaxBooksAllowed,
            member.MembershipDate,
            member.MembershipExpiryDate,
            member.IsActive
        );
    }
}