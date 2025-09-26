using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Membership.Contracts.Services;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Implementation of representative office validation service
/// Uses cache service for better performance when validating office existence
/// </summary>
public sealed class RepresentativeOfficeValidationService : IRepresentativeOfficeValidationService
{
    private readonly IRepresentativeOfficeService _representativeOfficeService;
    private readonly IBasicDefinitionsCacheService _cacheService;
    private readonly ILogger<RepresentativeOfficeValidationService> _logger;

    public RepresentativeOfficeValidationService(
        IRepresentativeOfficeService representativeOfficeService,
        IBasicDefinitionsCacheService cacheService,
        ILogger<RepresentativeOfficeValidationService> logger)
    {
        _representativeOfficeService = representativeOfficeService ?? throw new ArgumentNullException(nameof(representativeOfficeService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ValidateOfficeExistsAsync(Guid representativeOfficeId)
    {
        try
        {
            if (representativeOfficeId == Guid.Empty)
                return false;

            // Use cache service for better performance
            return await _cacheService.RepresentativeOfficeExistsAsync(representativeOfficeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating representative office existence for ID {OfficeId}", representativeOfficeId);
            return false;
        }
    }

    public async Task<Dictionary<Guid, bool>> ValidateOfficesExistAsync(IEnumerable<Guid> representativeOfficeIds)
    {
        var result = new Dictionary<Guid, bool>();
        
        try
        {
            if (representativeOfficeIds == null || !representativeOfficeIds.Any())
                return result;

            // Validate each office ID using cache service
            foreach (var officeId in representativeOfficeIds)
            {
                if (officeId == Guid.Empty)
                {
                    result[officeId] = false;
                    continue;
                }

                try
                {
                    result[officeId] = await _cacheService.RepresentativeOfficeExistsAsync(officeId);
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

    public async Task<RepresentativeOfficeBasicInfo?> GetOfficeBasicInfoAsync(Guid representativeOfficeId)
    {
        try
        {
            if (representativeOfficeId == Guid.Empty)
                return null;

            // Use cache service for better performance
            var office = await _cacheService.GetRepresentativeOfficeAsync(representativeOfficeId);
            if (office == null)
                return null;

            return new RepresentativeOfficeBasicInfo
            {
                Id = office.Id,
                Code = office.Code,
                Name = office.Name,
                IsActive = office.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting basic info for representative office ID {OfficeId}", representativeOfficeId);
            return null;
        }
    }
}
