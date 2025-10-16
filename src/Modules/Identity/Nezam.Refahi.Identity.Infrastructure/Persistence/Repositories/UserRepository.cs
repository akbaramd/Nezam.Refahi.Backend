using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

  /// <summary>
  /// Implementation of user repository interface using EF Core
  /// </summary>
  public class UserRepository : EfRepository<IdentityDbContext,User,Guid>, IUserRepository
  {
      public UserRepository(IdentityDbContext dbContext) : base(dbContext)
      {
      }

  public async Task<int> GetActiveUserCountAsync()
  {
    return await PrepareQuery(_dbSet).CountAsync(u => u.IsActive);
  }

  public async Task<IEnumerable<User>> GetActiveUsersAsync()
  {
    return await PrepareQuery(_dbSet).Where(u => u.IsActive).ToListAsync();
  }

  public async Task<IEnumerable<User>> GetByAuthenticationDateRangeAsync(DateTime fromDate, DateTime toDate)
  {
    return await PrepareQuery(_dbSet).Where(u => u.LastAuthenticatedAt >= fromDate && u.LastAuthenticatedAt <= toDate).ToListAsync();
  }



  public Task<IEnumerable<User>> GetByIpAddressAsync(string ipAddress)
  {
    throw new NotImplementedException();
  }

  public async Task<User?> GetByNationalIdAsync(NationalId nationalId)
      {
          return await PrepareQuery(_dbSet)
              .FirstOrDefaultAsync(u => u.NationalId != null && u.NationalId.Value == nationalId.Value);
      }

      public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
      {
        return await  PrepareQuery(_dbSet)
          .FirstOrDefaultAsync(u => u.PhoneNumber.Value == phoneNumber);
      }

    public async Task<User?> GetByPhoneNumberValueObjectAsync(PhoneNumber phoneNumber)
  {

return await PrepareQuery(_dbSet)
.FirstOrDefaultAsync(u => u.PhoneNumber.Value == phoneNumber.Value);
  }

  public async Task<IEnumerable<User>> GetByPhoneVerificationDateRangeAsync(DateTime fromDate, DateTime toDate)
  {
    return await PrepareQuery(_dbSet)
    .Where(u => u.PhoneVerifiedAt >= fromDate && u.PhoneVerifiedAt <= toDate)
    .ToListAsync();
  }

  public async Task<IEnumerable<User>> GetInactiveUsersAsync(DateTime beforeDate)
  {
    return await PrepareQuery(_dbSet)
    .Where(u => u.LastAuthenticatedAt < beforeDate)
    .ToListAsync();
  }

  public async Task<int> GetLockedUserCountAsync()
  {
    return await PrepareQuery(_dbSet).CountAsync(u => u.LockedAt != null);
  }

  public async Task<IEnumerable<User>> GetLockedUsersAsync()
  {
    return await PrepareQuery(_dbSet).Where(u => u.LockedAt != null).ToListAsync();
  }

  public async Task<IEnumerable<User>> GetUnverifiedUsersAsync()
  {
    return await PrepareQuery(_dbSet).Where(u => !u.IsPhoneVerified).ToListAsync();
  }

  public async Task<IEnumerable<User>> GetUsersToUnlockAsync()
  {
    return await PrepareQuery(_dbSet).Where(u => u.LockedAt != null && u.LockedAt < DateTime.UtcNow).ToListAsync();
  }

  public async Task<int> GetVerifiedUserCountAsync()
  {
    return await PrepareQuery(_dbSet).CountAsync(u => u.IsPhoneVerified);
  }

  public async Task<IEnumerable<User>> GetVerifiedUsersAsync()
  {
    return await PrepareQuery(_dbSet).Where(u => u.IsPhoneVerified).ToListAsync();
  }

  public async Task<bool> IsPhoneNumberRegisteredAsync(PhoneNumber phoneNumber)
  {
    return await PrepareQuery(_dbSet).AnyAsync(u => u.PhoneNumber.Value == phoneNumber.Value);
  }

  protected override IQueryable<User> PrepareQuery(IQueryable<User> query)
  {
    query = query.Include(u => u.UserRoles)
    .ThenInclude(ur => ur.Role)
    .ThenInclude(ur => ur.Claims)
    .Include(u => u.UserClaims)
    .Include(u => u.Preferences);
    return base.PrepareQuery(query);
  }

  public async Task<User?> GetByExternalIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
  {
    return await PrepareQuery(_dbSet)
      .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId, cancellationToken);
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(email)) return null;
    var normalized = email.Trim();
    return await PrepareQuery(_dbSet)
      .FirstOrDefaultAsync(u => u.Email != null && u.Email == normalized, cancellationToken);
  }
}
