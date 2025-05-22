using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

/// <summary>
/// Repository interface for SurveyResponse aggregate
/// </summary>
public interface ISurveyResponseRepository : IGenericRepository<SurveyResponse>
{
  /// <summary>
  /// Gets responses for a specific survey
  /// </summary>
  /// <param name="surveyId">The ID of the survey</param>
  /// <returns>Responses for the specified survey</returns>
  Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId);

  /// <summary>
  /// Gets responses submitted by a specific user
  /// </summary>
  /// <param name="responderId">The ID of the responder</param>
  /// <returns>Responses submitted by the specified user</returns>
  Task<IEnumerable<SurveyResponse>> GetByResponderIdAsync(Guid responderId);

  /// <summary>
  /// Gets a user's response to a specific survey
  /// </summary>
  /// <param name="surveyId">The ID of the survey</param>
  /// <param name="responderId">The ID of the responder</param>
  /// <returns>The user's response if found, null otherwise</returns>
  Task<SurveyResponse?> GetBySurveyAndResponderAsync(Guid surveyId, Guid responderId);

  /// <summary>
  /// Gets responses that have been submitted (not in progress)
  /// </summary>
  /// <param name="surveyId">The ID of the survey</param>
  /// <returns>Submitted responses for the specified survey</returns>
  Task<IEnumerable<SurveyResponse>> GetSubmittedResponsesAsync(Guid surveyId);

  /// <summary>
  /// Gets responses that are in progress (not yet submitted)
  /// </summary>
  /// <param name="surveyId">The ID of the survey</param>
  /// <returns>In-progress responses for the specified survey</returns>
  Task<IEnumerable<SurveyResponse>> GetInProgressResponsesAsync(Guid surveyId);
}
