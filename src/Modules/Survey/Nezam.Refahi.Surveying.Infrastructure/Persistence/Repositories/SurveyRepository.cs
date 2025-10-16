using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Infrastructure.Persistence;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Survey entity
/// </summary>
public class SurveyRepository : EfRepository<SurveyDbContext, Survey, Guid>, ISurveyRepository
{
    public SurveyRepository(SurveyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Survey>> GetByStateAsync(SurveyState state, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(s => s.State == state)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetActiveSurveysAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(s => s.State == SurveyState.Active &&
                       (!s.StartAt.HasValue || s.StartAt.Value <= now) &&
                       (!s.EndAt.HasValue || s.EndAt.Value >= now))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetByFeatureCodeAsync(string featureCode, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(s => s.SurveyFeatures.Any(sf => sf.FeatureCode == featureCode))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetByCapabilityCodeAsync(string capabilityCode, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(s => s.SurveyCapabilities.Any(sc => sc.CapabilityCode == capabilityCode))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Survey?> GetWithQuestionsAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == surveyId, cancellationToken);
    }

    public async Task<Survey?> GetWithResponsesAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Responses)
            .FirstOrDefaultAsync(s => s.Id == surveyId, cancellationToken);
    }

    public async Task<Survey?> GetWithAllDataAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
                .ThenInclude(r => r.QuestionAnswers)
                    .ThenInclude(qa => qa.SelectedOptions)
            .Include(s => s.SurveyFeatures)
            .Include(s => s.SurveyCapabilities)
            .FirstOrDefaultAsync(s => s.Id == surveyId, cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetInDateRangeAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetScheduledSurveysToActivateAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(s => s.State == SurveyState.Scheduled &&
                       s.StartAt.HasValue &&
                       s.StartAt.Value <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Survey>> GetActiveSurveysToCloseAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(s => s.State == SurveyState.Active &&
                       s.EndAt.HasValue &&
                       s.EndAt.Value < now)
            .ToListAsync(cancellationToken);
    }

    public async Task<Survey?> GetSurveyByResponseIdAsync(Guid responseId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Responses)
            .FirstOrDefaultAsync(s => s.Responses.Any(r => r.Id == responseId), cancellationToken);
    }

    public async Task<Survey?> GetSurveyWithResponseAndQuestionsAsync(Guid responseId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
                .ThenInclude(r => r.QuestionAnswers)
                    .ThenInclude(qa => qa.SelectedOptions)
            .FirstOrDefaultAsync(s => s.Responses.Any(r => r.Id == responseId), cancellationToken);
    }

    public async Task<Survey?> GetSurveyWithResponseAndAllDataAsync(Guid responseId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
                .ThenInclude(r => r.QuestionAnswers)
                    .ThenInclude(qa => qa.SelectedOptions)
            .Include(s => s.SurveyFeatures)
            .Include(s => s.SurveyCapabilities)
            .FirstOrDefaultAsync(s => s.Responses.Any(r => r.Id == responseId), cancellationToken);
    }

    public async Task<(IEnumerable<Survey> Surveys, int TotalCount)> GetSurveysWithPaginationAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        SurveyState? state = null,
        bool? isAnonymous = null,
        bool? isAcceptingResponses = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        DateTime? endDateFrom = null,
        DateTime? endDateTo = null,
        List<string>? requiredFeatures = null,
        List<string>? requiredCapabilities = null,
        List<string>? excludedFeatures = null,
        List<string>? excludedCapabilities = null,
        string sortBy = "CreatedAt",
        string sortDirection = "Desc",
        bool includeQuestions = false,
        bool includeResponses = false,
        CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => s.Title.Contains(searchTerm) || (s.Description != null && s.Description.Contains(searchTerm)));
        }

        if (state.HasValue)
        {
            query = query.Where(s => s.State == state.Value);
        }

        if (isAnonymous.HasValue)
        {
            query = query.Where(s => s.IsAnonymous == isAnonymous.Value);
        }

        if (isAcceptingResponses.HasValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (isAcceptingResponses.Value)
            {
                // Survey is accepting responses if it's active and within time window
                query = query.Where(s => s.State == SurveyState.Active &&
                                       (!s.StartAt.HasValue || s.StartAt.Value <= now) &&
                                       (!s.EndAt.HasValue || s.EndAt.Value >= now));
            }
            else
            {
                // Survey is not accepting responses if it's not active or outside time window
                query = query.Where(s => s.State != SurveyState.Active ||
                                       (s.StartAt.HasValue && s.StartAt.Value > now) ||
                                       (s.EndAt.HasValue && s.EndAt.Value < now));
            }
        }

        if (startDateFrom.HasValue)
        {
            query = query.Where(s => s.StartAt >= startDateFrom.Value);
        }

        if (startDateTo.HasValue)
        {
            query = query.Where(s => s.StartAt <= startDateTo.Value);
        }

        if (endDateFrom.HasValue)
        {
            query = query.Where(s => s.EndAt >= endDateFrom.Value);
        }

        if (endDateTo.HasValue)
        {
            query = query.Where(s => s.EndAt <= endDateTo.Value);
        }

        if (requiredFeatures != null && requiredFeatures.Any())
        {
            query = query.Where(s => requiredFeatures.All(f => s.SurveyFeatures.Any(sf => sf.FeatureCode == f)));
        }

        if (requiredCapabilities != null && requiredCapabilities.Any())
        {
            query = query.Where(s => requiredCapabilities.All(c => s.SurveyCapabilities.Any(sc => sc.CapabilityCode == c)));
        }

        if (excludedFeatures != null && excludedFeatures.Any())
        {
            query = query.Where(s => !s.SurveyFeatures.Any(sf => excludedFeatures.Contains(sf.FeatureCode)));
        }

        if (excludedCapabilities != null && excludedCapabilities.Any())
        {
            query = query.Where(s => !s.SurveyCapabilities.Any(sc => excludedCapabilities.Contains(sc.CapabilityCode)));
        }

        // Apply sorting
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            sortBy = "CreatedAt";
        }
        
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            sortDirection = "Desc";
        }

        query = sortDirection.ToLower() == "desc" 
            ? query.OrderByDescending(s => EF.Property<object>(s, sortBy))
            : query.OrderBy(s => EF.Property<object>(s, sortBy));

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var surveys = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (surveys, totalCount);
    }

    public async Task<(IEnumerable<Survey> Surveys, int TotalCount)> GetSurveysWithUserResponsesAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        SurveyState? state = null,
        bool? isAcceptingResponses = null,
        bool? hasUserResponse = null,
        bool? canUserParticipate = null,
        AttemptStatus? userResponseStatus = null,
        bool? userHasCompletedSurvey = null,
        decimal? minUserCompletionPercentage = null,
        decimal? maxUserCompletionPercentage = null,
        string sortBy = "CreatedAt",
        string sortDirection = "Desc",
        bool includeQuestions = false,
        bool includeUserResponses = true,
        bool includeUserLastResponse = true,
        CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => s.Title.Contains(searchTerm) || (s.Description != null && s.Description.Contains(searchTerm)));
        }

        if (state.HasValue)
        {
            query = query.Where(s => s.State == state.Value);
        }

        if (isAcceptingResponses.HasValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (isAcceptingResponses.Value)
            {
                // Survey is accepting responses if it's active and within time window
                query = query.Where(s => s.State == SurveyState.Active &&
                                       (!s.StartAt.HasValue || s.StartAt.Value <= now) &&
                                       (!s.EndAt.HasValue || s.EndAt.Value >= now));
            }
            else
            {
                // Survey is not accepting responses if it's not active or outside time window
                query = query.Where(s => s.State != SurveyState.Active ||
                                       (s.StartAt.HasValue && s.StartAt.Value > now) ||
                                       (s.EndAt.HasValue && s.EndAt.Value < now));
            }
        }

        if (hasUserResponse.HasValue)
        {
            if (hasUserResponse.Value)
            {
                query = query.Where(s => s.Responses.Any(r => r.Participant.MemberId == userId));
            }
            else
            {
                query = query.Where(s => !s.Responses.Any(r => r.Participant.MemberId == userId));
            }
        }

        if (userResponseStatus.HasValue)
        {
            query = query.Where(s => s.Responses.Any(r => r.Participant.MemberId == userId && r.AttemptStatus == userResponseStatus.Value));
        }

        if (userHasCompletedSurvey.HasValue)
        {
            if (userHasCompletedSurvey.Value)
            {
                query = query.Where(s => s.Responses.Any(r => r.Participant.MemberId == userId && r.SubmittedAt.HasValue));
            }
            else
            {
                query = query.Where(s => !s.Responses.Any(r => r.Participant.MemberId == userId && r.SubmittedAt.HasValue));
            }
        }

        // Apply sorting
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            sortBy = "CreatedAt";
        }
        
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            sortDirection = "Desc";
        }

        query = sortDirection.ToLower() == "desc" 
            ? query.OrderByDescending(s => EF.Property<object>(s, sortBy))
            : query.OrderBy(s => EF.Property<object>(s, sortBy));

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var surveys = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (surveys, totalCount);
    }

    public async Task<(int ActiveCount, int ScheduledCount, int ClosedCount, int ArchivedCount)> GetSurveyStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        var activeCount = await _dbSet.CountAsync(s => s.State == SurveyState.Active, cancellationToken);
        var scheduledCount = await _dbSet.CountAsync(s => s.State == SurveyState.Scheduled, cancellationToken);
        var closedCount = await _dbSet.CountAsync(s => s.State == SurveyState.Closed, cancellationToken);
        var archivedCount = await _dbSet.CountAsync(s => s.State == SurveyState.Archived, cancellationToken);

        return (activeCount, scheduledCount, closedCount, archivedCount);
    }

    public async Task<IEnumerable<Survey>> GetSurveysWithQuestionsAsync(
        IEnumerable<Guid> surveyIds,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(s => surveyIds.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<Survey> PrepareQuery(IQueryable<Survey> query)
    {
        return query
            .Include(s => s.Responses)
            .ThenInclude(x=>x.Participant)
            .Include(s => s.Responses)
            .ThenInclude(x=>x.QuestionAnswers)
            .ThenInclude(x=>x.SelectedOptions)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .Include(s => s.SurveyFeatures)
            .Include(s => s.SurveyCapabilities);
    }
}
