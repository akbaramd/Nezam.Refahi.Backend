using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of user repository interface using EF Core
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> GetByNationalIdAsync(NationalId nationalId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.NationalId.Value == nationalId.Value);
        }
    }
}
