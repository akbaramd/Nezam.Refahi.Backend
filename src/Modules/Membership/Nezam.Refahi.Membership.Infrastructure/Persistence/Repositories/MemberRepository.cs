using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of member repository interface using EF Core
/// </summary>
public class MemberRepository : EfRepository<MembershipDbContext, Member, Guid>, IMemberRepository


{
    public MemberRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Member?> GetByNationalCodeAsync(NationalId nationalCode)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .FirstOrDefaultAsync(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
    }

    public async Task<Member?> GetByMembershipNumberAsync(string membershipNumber)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .FirstOrDefaultAsync(m => m.MembershipNumber == membershipNumber);
    }

    public async Task<Member?> GetByPhoneNumberAsync(PhoneNumber phoneNumber)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .FirstOrDefaultAsync(m => m.PhoneNumber != null && m.PhoneNumber.Value == phoneNumber.Value);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .FirstOrDefaultAsync(m => m.Email == email.ToLowerInvariant());
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetByNameAsync(string firstName, string lastName)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Where(m => m.FullName.FirstName == firstName && m.FullName.LastName == lastName)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> SearchByNameAsync(string searchTerm)
    {
        var searchTermLower = searchTerm.ToLowerInvariant();
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Where(m => 
                m.FullName.FirstName.ToLower().Contains(searchTermLower) ||
                m.FullName.LastName.ToLower().Contains(searchTermLower) ||
                (m.FullName.FirstName + " " + m.FullName.LastName).ToLower().Contains(searchTermLower))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }


    public async Task<bool> IsNationalCodeExistsAsync(NationalId nationalCode, Guid? excludeMemberId = null)
    {
        var query = this.PrepareQuery(this._dbSet.AsQueryable()).Where(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
        
        if (excludeMemberId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMemberId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> IsMembershipNumberExistsAsync(string membershipNumber, Guid? excludeMemberId = null)
    {
        var query = this.PrepareQuery(this._dbSet.AsQueryable()).Where(m => m.MembershipNumber == membershipNumber);
        
        if (excludeMemberId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMemberId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<(IEnumerable<Member> Members, int TotalCount)> GetMembersPaginatedAsync(
        int pageNumber, 
        int pageSize)
    {
        var query = this.PrepareQuery(this._dbSet.AsQueryable());
        

        
        var totalCount = await query.CountAsync();
        
        var members = await query
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (members, totalCount);
    }

    public async Task<IEnumerable<Member>> GetByNationalCodesAsync(IEnumerable<NationalId> nationalCodes)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Where(x => nationalCodes.Any(v => x.NationalCode != null && v.Value == x.NationalCode.Value))
            .ToListAsync();
    }

    public async Task<Member?> GetByNationalCodeWithClaimsAsync(NationalId nationalCode)
    {
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Include(m => m.Claims)
            .FirstOrDefaultAsync(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
    }

    public async Task<IEnumerable<Member>> GetMembersWithExpiringClaimsAsync(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        var now = DateTimeOffset.UtcNow;
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Include(m => m.Claims)
            .Where(m =>
                       m.Claims.Any(c => c.ValidTo.HasValue && 
                                        c.ValidTo <= cutoffDate))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByClaimAsync(string claimType, string claimValue)
    {
        var now = DateTimeOffset.UtcNow;
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Include(m => m.Claims)
            .Where(m => 
                       m.Claims.Any(c => (!c.ValidTo.HasValue || c.ValidTo > now) &&
                                        (!c.ValidFrom.HasValue || c.ValidFrom <= now) &&
                                        c.Value == claimValue))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByClaimTypeAsync(string claimType)
    {
        var now = DateTimeOffset.UtcNow;
        return await this.PrepareQuery(this._dbSet.AsQueryable())
            .Include(m => m.Claims)
            .Where(m => 
                       m.Claims.Any(c => (!c.ValidTo.HasValue || c.ValidTo > now) &&
                                        (!c.ValidFrom.HasValue || c.ValidFrom <= now)))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }
}