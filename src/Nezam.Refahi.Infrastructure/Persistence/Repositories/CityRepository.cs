using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

  /// <summary>
  /// Implementation of city repository interface using EF Core
  /// </summary>
  public class CityRepository : GenericRepository<City>, ICityRepository
  {
      public CityRepository(ApplicationDbContext dbContext) : base(dbContext)
      {
      }

      public async Task<IEnumerable<City>> GetByProvinceIdAsync(Guid provinceId)
      {
          return await _dbSet
              .Where(c => c.ProvinceId == provinceId)
              .OrderBy(c => c.Name)
              .ToListAsync();
      }

      public async Task<IEnumerable<City>> FindByNameAsync(string name)
      {
          if (string.IsNullOrEmpty(name))
              return new List<City>();

          return await _dbSet
              .Where(c => c.Name.Contains(name))
              .OrderBy(c => c.Name)
              .ToListAsync();
      }

      public async Task<IEnumerable<City>> FindByPostalCodeAsync(string postalCode)
      {
          if (string.IsNullOrEmpty(postalCode))
              return new List<City>();

          return await _dbSet
              .Where(c => c.PostalCode != null && c.PostalCode.StartsWith(postalCode))
              .OrderBy(c => c.Name)
              .ToListAsync();
      }
  }
