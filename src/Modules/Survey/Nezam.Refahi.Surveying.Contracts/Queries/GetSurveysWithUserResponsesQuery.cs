using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get surveys with pagination, search, and user response information
/// </summary>
public class GetSurveysWithUserResponsesQuery : IRequest<ApplicationResult<SurveysWithUserResponsesResponse>>
{
    // User information
    public Guid? UserId { get; set; }
    public Dictionary<string, object>? UserDemographics { get; set; }
    
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    // Search and filtering
    public string? SearchTerm { get; set; }
    public string? State { get; set; } // Draft, Scheduled, Active, Closed, Archived
    public bool? IsAnonymous { get; set; }
    public bool? IsAcceptingResponses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    
    // Feature and capability filtering
    public List<string>? RequiredFeatures { get; set; }
    public List<string>? RequiredCapabilities { get; set; }
    public List<string>? ExcludedFeatures { get; set; }
    public List<string>? ExcludedCapabilities { get; set; }
    
    // User participation filtering
    public bool? HasUserResponse { get; set; }
    public bool? CanUserParticipate { get; set; }
    public string? UserResponseStatus { get; set; } // Active, Submitted, Canceled, Expired
    
    // Sorting
    public string SortBy { get; set; } = "CreatedAt"; // CreatedAt, Title, StartAt, EndAt, ResponseCount
    public string SortDirection { get; set; } = "Desc"; // Asc, Desc
    
    // Include options
    public bool IncludeQuestions { get; set; } = false;
    public bool IncludeUserResponses { get; set; } = true;
    public bool IncludeUserLastResponse { get; set; } = true;
    public bool IncludeStatistics { get; set; } = true;
    public bool IncludeFeatures { get; set; } = true;
    public bool IncludeCapabilities { get; set; } = true;
    
    // Response filtering
    public bool? UserHasCompletedSurvey { get; set; }
    public decimal? MinUserCompletionPercentage { get; set; }
    public decimal? MaxUserCompletionPercentage { get; set; }
}
