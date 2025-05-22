using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of survey question repository interface using EF Core
/// </summary>
public class SurveyQuestionRepository : GenericRepository<SurveyQuestion>, ISurveyQuestionRepository
{
    public SurveyQuestionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Gets questions for a specific survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Questions for the specified survey</returns>
    public async Task<IEnumerable<SurveyQuestion>> GetBySurveyIdAsync(Guid surveyId)
    {
        return await _dbSet
            .Where(q => q.SurveyId == surveyId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets questions for a specific survey ordered by their position
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Ordered questions for the specified survey</returns>
    public async Task<IEnumerable<SurveyQuestion>> GetOrderedBySurveyIdAsync(Guid surveyId)
    {
        return await _dbSet
            .Where(q => q.SurveyId == surveyId)
            .OrderBy(q => q.Order)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets questions of a specific type for a survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <param name="questionType">The type of questions to retrieve</param>
    /// <returns>Questions of the specified type for the survey</returns>
    public async Task<IEnumerable<SurveyQuestion>> GetByTypeAsync(Guid surveyId, QuestionType questionType)
    {
        return await _dbSet
            .Where(q => q.SurveyId == surveyId && q.Type == questionType)
            .OrderBy(q => q.Order)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets required questions for a survey
    /// </summary>
    /// <param name="surveyId">The ID of the survey</param>
    /// <returns>Required questions for the specified survey</returns>
    public async Task<IEnumerable<SurveyQuestion>> GetRequiredQuestionsAsync(Guid surveyId)
    {
        return await _dbSet
            .Where(q => q.SurveyId == surveyId && q.IsRequired)
            .OrderBy(q => q.Order)
            .ToListAsync();
    }
}
