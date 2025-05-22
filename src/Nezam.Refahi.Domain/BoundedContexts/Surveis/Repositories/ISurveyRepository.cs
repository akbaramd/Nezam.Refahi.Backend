using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

/// <summary>
/// Repository interface for Survey aggregate root
/// </summary>
public interface ISurveyRepository : IGenericRepository<Survey>
{
    /// <summary>
    /// Gets surveys by their status
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <returns>Surveys with the specified status</returns>
    Task<IEnumerable<Survey>> GetByStatusAsync(SurveyStatus status);
    
    /// <summary>
    /// Gets surveys created by a specific user
    /// </summary>
    /// <param name="creatorId">The ID of the creator</param>
    /// <returns>Surveys created by the specified user</returns>
    Task<IEnumerable<Survey>> GetByCreatorAsync(Guid creatorId);
    
    /// <summary>
    /// Gets currently active surveys (published and within their open/close dates)
    /// </summary>
    /// <param name="currentTimeUtc">The current UTC time</param>
    /// <returns>Active surveys</returns>
    Task<IEnumerable<Survey>> GetActiveAsync(DateTimeOffset currentTimeUtc);
}
