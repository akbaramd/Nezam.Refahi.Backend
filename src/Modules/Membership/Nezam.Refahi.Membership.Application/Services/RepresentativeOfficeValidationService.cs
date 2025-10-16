using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Membership.Contracts.Services;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Implementation of representative office validation service
/// Uses cache service for better performance when validating office existence
/// </summary>
public sealed class AgencyValidationService : IAgencyValidationService
{
    private readonly IAgencyService _AgencyService;
    private readonly IBasicDefinitionsCacheService _cacheService;
    private readonly ILogger<AgencyValidationService> _logger;

    public AgencyValidationService(
        IAgencyService AgencyService,
        IBasicDefinitionsCacheService cacheService,
        ILogger<AgencyValidationService> logger)
    {
        _AgencyService = AgencyService ?? throw new ArgumentNullException(nameof(AgencyService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ValidateOfficeExistsAsync(Guid AgencyId)
    {
        try
        {
            if (AgencyId == Guid.Empty)
                return false;

            // Use cache service for better performance
            return await _cacheService.AgencyExistsAsync(AgencyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating representative office existence for ID {OfficeId}", AgencyId);
            return false;
        }
    }

    public async Task<Dictionary<Guid, bool>> ValidateOfficesExistAsync(IEnumerable<Guid> AgencyIds)
    {
        var result = new Dictionary<Guid, bool>();
        
        try
        {
            if (AgencyIds == null || !AgencyIds.Any())
                return result;

            // Validate each office ID using cache service
            foreach (var officeId in AgencyIds)
            {
                if (officeId == Guid.Empty)
                {
                    result[officeId] = false;
                    continue;
                }

                try
                {
                    result[officeId] = await _cacheService.AgencyExistsAsync(officeId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating representative office existence for ID {OfficeId}", officeId);
                    result[officeId] = false;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating multiple representative offices");
            return result;
        }
    }

    public async Task<AgencyBasicInfo?> GetOfficeBasicInfoAsync(Guid AgencyId)
    {
        try
        {
            if (AgencyId == Guid.Empty)
                return null;

            // Use cache service for better performance
            var office = await _cacheService.GetAgencyAsync(AgencyId);
            if (office == null)
                return null;

            return new AgencyBasicInfo
            {
                Id = office.Id,
                Code = office.Code,
                Name = office.Name,
                IsActive = office.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting basic info for representative office ID {OfficeId}", AgencyId);
            return null;
        }
    }
}
