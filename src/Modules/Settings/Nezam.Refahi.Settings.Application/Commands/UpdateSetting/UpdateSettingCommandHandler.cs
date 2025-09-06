using FluentValidation;
using MediatR;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.Services;
using Nezam.Refahi.Settings.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.UpdateSetting;

/// <summary>
/// Handler for the UpdateSettingCommand
/// </summary>
public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand, ApplicationResult<UpdateSettingResponse>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SettingsDomainService _domainService;
    private readonly IValidator<UpdateSettingCommand> _validator;
    private readonly ISettingsUnitOfWork _unitOfWork;

    public UpdateSettingCommandHandler(
        ISettingsRepository settingsRepository,
        SettingsDomainService domainService,
        IValidator<UpdateSettingCommand> validator,
        ISettingsUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<UpdateSettingResponse>> Handle(
        UpdateSettingCommand request,
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
                return ApplicationResult<UpdateSettingResponse>.Failure(errors, "Validation failed");
            }

            // Get the existing setting
            var setting = await _settingsRepository.GetSettingByIdAsync(request.SettingId);
            if (setting == null)
            {
                return ApplicationResult<UpdateSettingResponse>.Failure(
                    new List<string> { "Setting not found" },
                    "Setting does not exist");
            }

            // Create new setting value with the same type
            var newValue = new SettingValue(request.NewValue, setting.Value.Type);

            // Apply the setting value change using domain service
            var changeEvent = await _domainService.ApplySettingValueChangeAsync(
                setting, newValue, request.UserId, request.ChangeReason);

            // Update the setting in the repository
            await _settingsRepository.UpdateSettingAsync(setting);

            await _unitOfWork.CommitAsync(cancellationToken);

            // Return response
            var response = new UpdateSettingResponse
            {
                SettingId = setting.Id,
                NewValue = request.NewValue,
                ChangeEventId = changeEvent.Id,
                UpdatedAt = DateTime.UtcNow,
                ChangedByUserId = request.UserId
            };

            return ApplicationResult<UpdateSettingResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<UpdateSettingResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to update setting");
        }
    }
}
