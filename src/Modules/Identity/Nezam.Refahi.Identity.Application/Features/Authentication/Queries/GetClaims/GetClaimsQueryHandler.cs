using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetClaims;

/// <summary>
/// Handler for getting all available claims from registered claim providers
/// </summary>
public class GetClaimsQueryHandler : IRequestHandler<GetClaimsQuery, ApplicationResult<IEnumerable<ClaimDto>>>
{
    private readonly IClaimsAggregatorService _claimsAggregatorService;
    private readonly ILogger<GetClaimsQueryHandler> _logger;

    public GetClaimsQueryHandler(
        IClaimsAggregatorService claimsAggregatorService,
        ILogger<GetClaimsQueryHandler> logger)
    {
        _claimsAggregatorService = claimsAggregatorService ?? throw new ArgumentNullException(nameof(claimsAggregatorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<IEnumerable<ClaimDto>>> Handle(
        GetClaimsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting all distinct claims from aggregator service");
            
            // Get all distinct claims from all registered providers
            var claims = await _claimsAggregatorService.GetAllDistinctClaimsAsync(cancellationToken);
            
            var claimList = claims.Select(claim=>new ClaimDto()
            {
              Value = claim.Value,
              Type = claim.Type,
              ValueType = claim.ValueType,
            }).ToList();
            
            _logger.LogInformation("Successfully retrieved {ClaimCount} distinct claims", claimList.Count);
            
            return ApplicationResult<IEnumerable<ClaimDto>>.Success(
                claimList, 
                $"Successfully retrieved {claimList.Count} distinct claims");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve claims from aggregator service");
            
            return ApplicationResult<IEnumerable<ClaimDto>>.Failure(
                "Failed to retrieve available claims. Please try again later.");
        }
    }
}