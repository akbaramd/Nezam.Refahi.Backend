using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

/// <summary>
/// Repository interface for SurveyOptions entity
/// </summary>
public interface ISurveyOptionsRepository : IGenericRepository<SurveyOptions>
{
    /// <summary>
    /// Gets options for a specific question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Options for the specified question</returns>
    Task<IEnumerable<SurveyOptions>> GetByQuestionIdAsync(Guid questionId);
    
    /// <summary>
    /// Gets options for a specific question ordered by their position
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Ordered options for the specified question</returns>
    Task<IEnumerable<SurveyOptions>> GetOrderedByQuestionIdAsync(Guid questionId);
    
    /// <summary>
    /// Gets the most selected options for a question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="limit">Maximum number of options to return</param>
    /// <returns>The most selected options</returns>
    Task<IEnumerable<SurveyOptions>> GetMostSelectedOptionsAsync(Guid questionId, int limit = 5);
}
