using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member> 
{
    Task<Member?> GetMemberWithDetailsAsync(Guid id);
    Task<bool> MembershipNumberExistsAsync(string number, Guid? excludeId = null);
    Task<Member?> GetMemberByUserIdAsync(string userId);
}