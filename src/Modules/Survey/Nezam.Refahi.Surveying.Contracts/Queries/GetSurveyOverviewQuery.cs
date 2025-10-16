using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get survey overview details for main page display
/// </summary>
public record GetSurveyOverviewQuery(Guid SurveyId) : IRequest<ApplicationResult<SurveyOverviewResponse>>;

/// <summary>
/// Response containing survey overview information
/// </summary>
public class SurveyOverviewResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string State { get; set; } = string.Empty;
    public string StateText { get; set; } = string.Empty;
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Participation policy
    public int MaxAttemptsPerMember { get; set; }
    public bool AllowMultipleSubmissions { get; set; }
    public int? CoolDownSeconds { get; set; }
    public bool AllowBackNavigation { get; set; }
    
    // Survey structure
    public int TotalQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public TimeSpan? EstimatedDuration { get; set; }
    
    // Features and capabilities
    public List<string> FeatureCodes { get; set; } = new();
    public List<string> CapabilityCodes { get; set; } = new();
    
    // Audience filter
    public string? AudienceFilter { get; set; }
    
    // Structure versioning
    public int StructureVersion { get; set; }
    public bool IsStructureFrozen { get; set; }
}
