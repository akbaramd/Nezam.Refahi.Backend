using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Simple query to get surveys with user's last response information
/// </summary>
public class GetSurveysWithUserLastResponseQuery : IRequest<ApplicationResult<SurveysWithUserLastResponseResponse>>
{
    // User information
    public string? NationalNumber { get; set; } // National number for member identification
    public Dictionary<string, object>? UserDemographics { get; set; }
    
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    // Basic filtering
    public string? SearchTerm { get; set; }
    public string? State { get; set; } // Active, Closed, etc.
    public bool? IsAcceptingResponses { get; set; }
    
    // Sorting
    public string SortBy { get; set; } = "CreatedAt"; // CreatedAt, Title, StartAt
    public string SortDirection { get; set; } = "Desc"; // Asc, Desc
    
    // Include options
    public bool IncludeQuestions { get; set; } = false;
    public bool IncludeUserLastResponse { get; set; } = true;
}
