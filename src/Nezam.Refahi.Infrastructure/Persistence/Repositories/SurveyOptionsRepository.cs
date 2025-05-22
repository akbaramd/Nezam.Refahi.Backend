using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of survey options repository interface using EF Core
/// </summary>
public class SurveyOptionsRepository : GenericRepository<SurveyOptions>, ISurveyOptionsRepository
{
    public SurveyOptionsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Gets options for a specific question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Options for the specified question</returns>
    public async Task<IEnumerable<SurveyOptions>> GetByQuestionIdAsync(Guid questionId)
    {
        return await AsDbSet()
            .Where(o => o.QuestionId == questionId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets options for a specific question ordered by their position
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>Ordered options for the specified question</returns>
    public async Task<IEnumerable<SurveyOptions>> GetOrderedByQuestionIdAsync(Guid questionId)
    {
        return await AsDbSet()
            .Where(o => o.QuestionId == questionId)
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets the most selected options for a question
    /// </summary>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="limit">Maximum number of options to return</param>
    /// <returns>The most selected options</returns>
    public async Task<IEnumerable<SurveyOptions>> GetMostSelectedOptionsAsync(Guid questionId, int limit = 5)
    {
        // This implementation counts the occurrences of each option in answers
        // for the specified question
        
        var optionSelectionCounts = await AsDbContext().Set<SurveyAnswer>()
            .Where(a => a.QuestionId == questionId && a.OptionId != null)
            .GroupBy(a => a.OptionId)
            .Select(g => new { OptionId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(limit)
            .ToListAsync();
            
        if (!optionSelectionCounts.Any())
            return new List<SurveyOptions>();
            
        var optionIds = optionSelectionCounts.Select(x => x.OptionId!.Value).ToList();
        
        // Get the actual options in the same order as the counts
        var options = await AsDbSet()
            .Where(o => optionIds.Contains(o.Id))
            .ToListAsync();
            
        // Order the options by selection count
        return options
            .OrderBy(o => optionSelectionCounts.FindIndex(x => x.OptionId == o.Id))
            .ToList();
    }
}
