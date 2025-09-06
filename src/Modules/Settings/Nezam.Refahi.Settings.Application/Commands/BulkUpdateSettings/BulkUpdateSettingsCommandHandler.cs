using FluentValidation;
using MediatR;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Handler for the BulkUpdateSettingsCommand
/// </summary>
public class BulkUpdateSettingsCommandHandler : IRequestHandler<BulkUpdateSettingsCommand, ApplicationResult<BulkUpdateSettingsResponse>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SettingsDomainService _domainService;
    private readonly IValidator<BulkUpdateSettingsCommand> _validator;
    private readonly ISettingsUnitOfWork _unitOfWork;

    public BulkUpdateSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        SettingsDomainService domainService,
        IValidator<BulkUpdateSettingsCommand> validator,
        ISettingsUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<BulkUpdateSettingsResponse>> Handle(
        BulkUpdateSettingsCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<BulkUpdateSettingsResponse>.Failure(errors, "Validation failed");
            }

            // Validate business rules using domain service
            var businessValidation = await _domainService.ValidateBulkSettingsUpdateAsync(
                request.SettingUpdates, request.UserId);

            if (!businessValidation.IsValid)
            {
                return ApplicationResult<BulkUpdateSettingsResponse>.Failure(
                    businessValidation.Errors,
                    "Business validation failed");
            }

            // Apply bulk updates using domain service
            var changeEvents = await _domainService.ApplyBulkSettingsUpdateAsync(
                request.SettingUpdates, request.UserId, request.ChangeReason);

            // Update all settings in the repository
            foreach (var changeEvent in changeEvents)
            {
                var setting = await _settingsRepository.GetSettingByIdAsync(changeEvent.SettingId);
                if (setting != null)
                {
                    await _settingsRepository.UpdateSettingAsync(setting);
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            // Build response
            var response = new BulkUpdateSettingsResponse
            {
                TotalProcessed = request.SettingUpdates.Count,
                SuccessfullyUpdated = changeEvents.Count,
                FailedToUpdate = request.SettingUpdates.Count - changeEvents.Count,
                ChangeEventsCreated = changeEvents.Count,
                CompletedAt = DateTime.UtcNow
            };

            // Add successful updates
            foreach (var changeEvent in changeEvents)
            {
                var setting = await _settingsRepository.GetSettingByIdAsync(changeEvent.SettingId);
                if (setting != null)
                {
                    response.SuccessfulUpdates.Add(new SuccessfulUpdate
                    {
                        SettingId = changeEvent.SettingId,
                        NewValue = changeEvent.NewValue.RawValue,
                        ChangeEventId = changeEvent.Id
                    });
                }
            }

            // Add failed updates (if any)
            var successfulIds = changeEvents.Select(e => e.SettingId).ToHashSet();
            foreach (var update in request.SettingUpdates)
            {
                if (!successfulIds.Contains(update.Key))
                {
                    response.FailedUpdates.Add(new FailedUpdate
                    {
                        SettingId = update.Key,
                        FailureReason = "Setting not found or cannot be updated"
                    });
                }
            }

            return ApplicationResult<BulkUpdateSettingsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<BulkUpdateSettingsResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to bulk update settings");
        }
    }
}
