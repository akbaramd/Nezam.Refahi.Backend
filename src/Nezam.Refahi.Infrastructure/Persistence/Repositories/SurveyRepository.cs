using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

  /// <summary>
  /// Implementation of survey repository interface using EF Core
  /// </summary>
  public class SurveyRepository : GenericRepository<Survey>, ISurveyRepository
  {
      public SurveyRepository(ApplicationDbContext dbContext) : base(dbContext)
      {
      }

      /// <summary>
      /// Gets surveys by their status
      /// </summary>
      /// <param name="status">The status to filter by</param>
      /// <returns>Surveys with the specified status</returns>
      public async Task<IEnumerable<Survey>> GetByStatusAsync(SurveyStatus status)
      {
          return await AsDbSet()
              .Where(s => s.Status == status)
              .ToListAsync();
      }

      /// <summary>
      /// Gets surveys created by a specific user
      /// </summary>
      /// <param name="creatorId">The ID of the creator</param>
      /// <returns>Surveys created by the specified user</returns>
      public async Task<IEnumerable<Survey>> GetByCreatorAsync(Guid creatorId)
      {
          return await AsDbSet()
              .Where(s => s.CreatorId == creatorId)
              .ToListAsync();
      }

      /// <summary>
      /// Gets currently active surveys (published and within their open/close dates)
      /// </summary>
      /// <param name="currentTimeUtc">The current UTC time</param>
      /// <returns>Active surveys</returns>
      public async Task<IEnumerable<Survey>> GetActiveAsync(DateTimeOffset currentTimeUtc)
      {
          return await AsDbSet()
              .Where(s => s.Status == SurveyStatus.Published &&
                         s.OpensAtUtc <= currentTimeUtc &&
                         (s.ClosesAtUtc == null || s.ClosesAtUtc >= currentTimeUtc))
              .ToListAsync();
      }
  }
