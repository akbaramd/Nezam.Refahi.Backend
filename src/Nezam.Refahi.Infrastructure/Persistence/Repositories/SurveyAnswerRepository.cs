using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of survey answer repository interface using EF Core
/// </summary>
public class SurveyAnswerRepository : GenericRepository<SurveyAnswer>, ISurveyAnswerRepository
{
    public SurveyAnswerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Gets answers for a specific survey response
    /// </summary>
    /// <param name="responseId">The ID of the survey response</param>
    /// <returns>Answers for the specified response</returns>
    public async Task<IEnumerable<SurveyAnswer>> GetByResponseIdAsync(Guid responseId)
    {
        return await AsDbSet()
            .Where(a => a.ResponseId == responseId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets answers for a specific question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Answers for the specified question</returns>
    public async Task<IEnumerable<SurveyAnswer>> GetByQuestionIdAsync(Guid questionId)
    {
        return await AsDbSet()
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets a specific answer in a response
    /// </summary>
    /// <param name="responseId">The ID of the response</param>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>The answer if found, null otherwise</returns>
    public async Task<SurveyAnswer?> GetByResponseAndQuestionAsync(Guid responseId, Guid questionId)
    {
        return await AsDbSet()
            .FirstOrDefaultAsync(a => a.ResponseId == responseId && a.QuestionId == questionId);
    }
    
    /// <summary>
    /// Gets all answers for a specific survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>All answers for the specified survey</returns>
    public async Task<IEnumerable<SurveyAnswer>> GetBySurveyIdAsync(Guid surveyId)
    {
        return await AsDbSet()
            .Include(a => a.Response)
            .Where(a => a.Response.SurveyId == surveyId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets answers with a specific selected option for a question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="optionId">The ID of the option</param>
    /// <returns>Answers that selected the specified option</returns>
    public async Task<IEnumerable<SurveyAnswer>> GetBySelectedOptionAsync(Guid questionId, Guid optionId)
    {
        return await AsDbSet()
            .Where(a => a.QuestionId == questionId && a.OptionId == optionId)
            .ToListAsync();
    }
}
