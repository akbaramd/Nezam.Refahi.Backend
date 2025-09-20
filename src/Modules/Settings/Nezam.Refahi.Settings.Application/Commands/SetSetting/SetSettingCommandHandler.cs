using FluentValidation;
using MediatR;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.Services;
using Nezam.Refahi.Settings.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.SetSetting;

/// <summary>
/// Handler for the SetSettingCommand
/// </summary>
public class SetSettingCommandHandler : IRequestHandler<SetSettingCommand, ApplicationResult<SetSettingResponse>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SettingsValidationService _validationService;
    private readonly IValidator<SetSettingCommand> _validator;
    private readonly ISettingsUnitOfWork _unitOfWork;

    public SetSettingCommandHandler(
        ISettingsRepository settingsRepository,
        SettingsValidationService validationService,
        IValidator<SetSettingCommand> validator,
        ISettingsUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<SetSettingResponse>> Handle(
        SetSettingCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellation:cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<SetSettingResponse>.Failure(errors, "Validation failed");
            }

            // Validate business rules
            var businessValidation = _validationService.ValidateSettingCreation(
                new SettingKey(request.Key),
                new SettingValue(request.Value, request.Type),
                request.Description,
                request.CategoryId);

            if (!businessValidation.IsValid)
            {
                return ApplicationResult<SetSettingResponse>.Failure(
                    businessValidation.Errors, 
                    "Business validation failed");
            }

            // Check if category exists
            var category = await _settingsRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null)
            {
                return ApplicationResult<SetSettingResponse>.Failure(
                    new List<string> { "Category not found" },
                    "Category does not exist");
            }

            // Check if setting already exists
            var existingSetting = await _settingsRepository.GetSettingByKeyAsync(request.Key);
            bool wasCreated = false;
            Guid? changeEventId = null;

            if (existingSetting != null)
            {
                // Update existing setting
                var newValue = new SettingValue(request.Value, request.Type);
                
                // Validate the value change
                var changeValidation = _validationService.ValidateSettingValueChange(
                    existingSetting.Value.RawValue, request.Value, existingSetting.Value.Type);
                
                if (!changeValidation.IsValid)
                {
                    return ApplicationResult<SetSettingResponse>.Failure(
                        changeValidation.Errors,
                        "Cannot update setting");
                }

                // Create change event
                var changeEvent = new SettingChangeEvent(
                    existingSetting.Id,
                    existingSetting.Key,
                    existingSetting.Value,
                    newValue,
                    request.UserId,
                    request.ChangeReason
                );

                // Update the setting
                existingSetting.UpdateValue(newValue, request.UserId, request.ChangeReason);
                existingSetting.ChangeEvents.Add(changeEvent);

                await _settingsRepository.UpdateSettingAsync(existingSetting);
                changeEventId = changeEvent.Id;
            }
            else
            {
                // Create new setting
                var settingKey = new SettingKey(request.Key);
                var settingValue = new SettingValue(request.Value, request.Type);
                
                var newSetting = new Setting(
                    settingKey,
                    settingValue,
                    request.Description,
                    request.CategoryId,
                    request.IsReadOnly,
                    request.DisplayOrder
                );

                await _settingsRepository.AddSettingAsync(newSetting);
                existingSetting = newSetting;
                wasCreated = true;
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            // Return response
            var response = new SetSettingResponse
            {
                Id = existingSetting.Id,
                Key = existingSetting.Key.Value,
                Value = existingSetting.Value.RawValue,
                Type = existingSetting.Value.Type,
                WasCreated = wasCreated,
                ChangeEventId = changeEventId,
                ModifiedAt = DateTime.UtcNow
            };

            return ApplicationResult<SetSettingResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<SetSettingResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to set setting");
        }
    }
}
