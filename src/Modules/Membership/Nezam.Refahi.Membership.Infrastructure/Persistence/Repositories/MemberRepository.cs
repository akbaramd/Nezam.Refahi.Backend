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
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
    }

    public async Task<Member?> GetByMembershipNumberAsync(string membershipNumber)
    {
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(m => m.MembershipNumber == membershipNumber);
    }

    public async Task<Member?> GetByPhoneNumberAsync(PhoneNumber phoneNumber)
    {
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(m => m.PhoneNumber != null && m.PhoneNumber.Value == phoneNumber.Value);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(m => m.Email == email.ToLowerInvariant());
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await this.PrepareQuery(this._dbSet)
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetByNameAsync(string firstName, string lastName)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(m => m.FullName.FirstName == firstName && m.FullName.LastName == lastName)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> SearchByNameAsync(string searchTerm)
    {
        var searchTermLower = searchTerm.ToLowerInvariant();
        return await this.PrepareQuery(this._dbSet)
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
        var query = this.PrepareQuery(this._dbSet).Where(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
        
        if (excludeMemberId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMemberId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> IsMembershipNumberExistsAsync(string membershipNumber, Guid? excludeMemberId = null)
    {
        var query = this.PrepareQuery(this._dbSet).Where(m => m.MembershipNumber == membershipNumber);
        
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
        var query = this.PrepareQuery(this._dbSet);
        

        
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
        return await this.PrepareQuery(this._dbSet)
            .Where(x => nationalCodes.Any(v => x.NationalCode != null && v.Value == x.NationalCode.Value))
            .ToListAsync();
    }

    public async Task<Member?> GetByNationalCodeWithCapabilitiesAsync(NationalId nationalCode)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
                    .ThenInclude(c => c.Features)
            .Include(m => m.Roles)
                .ThenInclude(mr => mr.Role)
            .FirstOrDefaultAsync(m => m.NationalCode != null && m.NationalCode.Value == nationalCode.Value);
    }

    public async Task<IEnumerable<Member>> GetMembersWithExpiringCapabilitiesAsync(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
            .Include(m => m.Roles)
                .ThenInclude(mr => mr.Role)
            .Where(m => m.Capabilities.Any(mc =>
                mc.ValidTo.HasValue &&
                mc.ValidTo.Value <= cutoffDate))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByCapabilityAsync(string capabilityId)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
            .Include(m => m.Roles)
                .ThenInclude(mr => mr.Role)
            .Where(m => m.Capabilities.Any(mc =>
                mc.CapabilityId == capabilityId &&
                mc.IsValid()))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByRoleAsync(Guid roleId)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
            .Include(m => m.Roles)
                .ThenInclude(mr => mr.Role)
            .Where(m => m.Roles.Any(mr =>
                mr.RoleId == roleId &&
                mr.IsValid()))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByFeatureAsync(string featureId)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
                    .ThenInclude(c => c.Features)
            .Where(m => m.Capabilities.Any(mc =>
                mc.IsValid() &&
                mc.Capability!.Features.Any(ct => ct.Id == featureId)))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<Member?> GetByIdWithCapabilitiesAsync(Guid id)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
                    .ThenInclude(c => c.Features)
            .Include(m => m.Roles)
                .ThenInclude(mr => mr.Role)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Member>> GetMembersWithoutCapabilitiesAsync()
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(m => !m.Capabilities.Any(mc => mc.IsActive))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithAllCapabilitiesAsync(IEnumerable<string> capabilityIds)
    {
        var capabilityIdsList = capabilityIds.ToList();
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
            .Where(m => capabilityIdsList.All(capId =>
                m.Capabilities.Any(mc =>
                    mc.CapabilityId == capId && mc.IsValid())))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithAnyCapabilitiesAsync(IEnumerable<string> capabilityIds)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(m => m.Capabilities)
                .ThenInclude(mc => mc.Capability)
            .Where(m => m.Capabilities.Any(mc =>
                capabilityIds.Contains(mc.CapabilityId) && mc.IsValid()))
            .OrderBy(m => m.FullName.FirstName)
            .ThenBy(m => m.FullName.LastName)
            .ToListAsync();
    }

    protected override IQueryable<Member> PrepareQuery(IQueryable<Member> query)
    {
      query = query.Include(x => x.Capabilities)
        .ThenInclude(x => x.Capability)
        .ThenInclude(x => x.Features);
      return base.PrepareQuery(query);
    }
}