using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

/// <summary>
/// Repository interface for SurveyQuestion entity
/// </summary>
public interface ISurveyQuestionRepository : IGenericRepository<SurveyQuestion>
{
    /// <summary>
    /// Gets questions for a specific survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Questions for the specified survey</returns>
    Task<IEnumerable<SurveyQuestion>> GetBySurveyIdAsync(Guid surveyId);
    
    /// <summary>
    /// Gets questions for a specific survey ordered by their position
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Ordered questions for the specified survey</returns>
    Task<IEnumerable<SurveyQuestion>> GetOrderedBySurveyIdAsync(Guid surveyId);
    
    /// <summary>
    /// Gets questions of a specific type for a survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <param name="questionType">The type of questions to retrieve</param>
    /// <returns>Questions of the specified type for the survey</returns>
    Task<IEnumerable<SurveyQuestion>> GetByTypeAsync(Guid surveyId, QuestionType questionType);
    
    /// <summary>
    /// Gets required questions for a survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Required questions for the specified survey</returns>
    Task<IEnumerable<SurveyQuestion>> GetRequiredQuestionsAsync(Guid surveyId);
}
