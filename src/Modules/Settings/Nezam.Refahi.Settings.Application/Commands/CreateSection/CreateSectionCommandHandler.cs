using FluentValidation;
using MediatR;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.CreateSection;

/// <summary>
/// Handler for the CreateSectionCommand
/// </summary>
public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, ApplicationResult<CreateSectionResponse>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SettingsValidationService _validationService;
    private readonly IValidator<CreateSectionCommand> _validator;
    private readonly ISettingsUnitOfWork _unitOfWork;

    public CreateSectionCommandHandler(
        ISettingsRepository settingsRepository,
        SettingsValidationService validationService,
        IValidator<CreateSectionCommand> validator,
        ISettingsUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CreateSectionResponse>> Handle(
        CreateSectionCommand request,
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
                return ApplicationResult<CreateSectionResponse>.Failure(errors, "Validation failed");
            }

            // Validate business rules
            var businessValidation = _validationService.ValidateSectionCreation(
                request.Name, 
                request.Description, 
                request.DisplayOrder);

            if (!businessValidation.IsValid)
            {
                return ApplicationResult<CreateSectionResponse>.Failure(
                    businessValidation.Errors, 
                    "Business validation failed");
            }

            // Check if section name is unique
            var isNameUnique = await _settingsRepository.SectionNameExistsAsync(request.Name);
            if (isNameUnique)
            {
                return ApplicationResult<CreateSectionResponse>.Failure(
                    new List<string> { "Section name already exists" },
                    "Section name must be unique");
            }

            // Create the section
            var section = new SettingsSection(request.Name, request.Description, request.DisplayOrder);
            await _settingsRepository.AddSectionAsync(section);

            await _unitOfWork.CommitAsync(cancellationToken);

            // Return response
            var response = new CreateSectionResponse
            {
                Id = section.Id,
                Name = section.Name,
                Description = section.Description,
                DisplayOrder = section.DisplayOrder,
                IsActive = section.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            return ApplicationResult<CreateSectionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreateSectionResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to create section");
        }
    }
}
