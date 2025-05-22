using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

/// <summary>
/// Repository interface for SurveyAnswer entity
/// </summary>
public interface ISurveyAnswerRepository : IGenericRepository<SurveyAnswer>
{
    /// <summary>
    /// Gets answers for a specific survey response
    /// </summary>
    /// <param name="responseId">The ID of the survey response</param>
    /// <returns>Answers for the specified response</returns>
    Task<IEnumerable<SurveyAnswer>> GetByResponseIdAsync(Guid responseId);
    
    /// <summary>
    /// Gets answers for a specific question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Answers for the specified question</returns>
    Task<IEnumerable<SurveyAnswer>> GetByQuestionIdAsync(Guid questionId);
    
    /// <summary>
    /// Gets a specific answer in a response
    /// </summary>
    /// <param name="responseId">The ID of the response</param>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>The answer if found, null otherwise</returns>
    Task<SurveyAnswer?> GetByResponseAndQuestionAsync(Guid responseId, Guid questionId);
    
    /// <summary>
    /// Gets all answers for a specific survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>All answers for the specified survey</returns>
    Task<IEnumerable<SurveyAnswer>> GetBySurveyIdAsync(Guid surveyId);
    
    /// <summary>
    /// Gets answers with selected options for a specific question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="optionId">The ID of the option</param>
    /// <returns>Answers that selected the specified option</returns>
    Task<IEnumerable<SurveyAnswer>> GetBySelectedOptionAsync(Guid questionId, Guid optionId);
}
