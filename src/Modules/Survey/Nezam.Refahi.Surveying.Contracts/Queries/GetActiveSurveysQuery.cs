using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get active surveys available for participation
/// </summary>
public class GetActiveSurveysQuery : IRequest<ApplicationResult<ActiveSurveysResponse>>
{
    public Guid? UserId { get; set; } // For checking participation eligibility
    public string? FeatureKey { get; set; } // Filter by feature
    public string? CapabilityKey { get; set; } // Filter by capability
    public bool IncludeQuestions { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
