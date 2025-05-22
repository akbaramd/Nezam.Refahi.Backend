using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

  /// <summary>
  /// Implementation of survey response repository interface using EF Core
  /// </summary>
  public class SurveyResponseRepository : GenericRepository<SurveyResponse>, ISurveyResponseRepository
  {
      public SurveyResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
      {
      }

      /// <summary>
      /// Gets responses for a specific survey
      /// </summary>
      /// <param name="surveyId">The ID of the survey</param>
      /// <returns>Responses for the specified survey</returns>
      public async Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId)
      {
          return await AsDbSet()
              .Where(r => r.SurveyId == surveyId)
              .ToListAsync();
      } 

      /// <summary>
      /// Gets responses submitted by a specific user
      /// </summary>
      /// <param name="responderId">The ID of the responder</param>
      /// <returns>Responses submitted by the specified user</returns>
      public async Task<IEnumerable<SurveyResponse>> GetByResponderIdAsync(Guid responderId)
      {
          return await AsDbSet()
              .Where(r => r.ResponderId == responderId)
              .ToListAsync();
      }

      /// <summary>
      /// Gets a user's response to a specific survey
      /// </summary>
      /// <param name="surveyId">The ID of the survey</param>
      /// <param name="responderId">The ID of the responder</param>
      /// <returns>The user's response if found, null otherwise</returns>
      public async Task<SurveyResponse?> GetBySurveyAndResponderAsync(Guid surveyId, Guid responderId)
      {
          return await AsDbSet()
              .FirstOrDefaultAsync(r => r.SurveyId == surveyId && r.ResponderId == responderId);
      }

      /// <summary>
      /// Gets responses that have been submitted (not in progress)
      /// </summary>
      /// <param name="surveyId">The ID of the survey</param>
      /// <returns>Submitted responses for the specified survey</returns>
      public async Task<IEnumerable<SurveyResponse>> GetSubmittedResponsesAsync(Guid surveyId)
      {
          return await AsDbSet()
              .Where(r => r.SurveyId == surveyId && r.SubmittedAtUtc != null)
              .ToListAsync();
      }

      /// <summary>
      /// Gets responses that are in progress (not yet submitted)
      /// </summary>
      /// <param name="surveyId">The ID of the survey</param>
      /// <returns>In-progress responses for the specified survey</returns>
      public async Task<IEnumerable<SurveyResponse>> GetInProgressResponsesAsync(Guid surveyId)
      {
          return await AsDbSet()
              .Where(r => r.SurveyId == surveyId && r.SubmittedAtUtc == null)
              .ToListAsync();
      }
  }
