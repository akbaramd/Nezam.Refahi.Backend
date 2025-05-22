using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of province repository interface using EF Core
    /// </summary>
    public class ProvinceRepository : GenericRepository<Province>, IProvinceRepository
    {
        public ProvinceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Province?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<IEnumerable<Province>> FindByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new List<Province>();

            return await _dbSet
                .Where(p => p.Name.Contains(name))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
