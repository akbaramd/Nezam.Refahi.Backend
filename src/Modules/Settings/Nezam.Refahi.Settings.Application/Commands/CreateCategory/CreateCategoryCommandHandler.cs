using FluentValidation;
using MediatR;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.CreateCategory;

/// <summary>
/// Handler for the CreateCategoryCommand
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApplicationResult<CreateCategoryResponse>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly SettingsValidationService _validationService;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly ISettingsUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ISettingsRepository settingsRepository,
        SettingsValidationService validationService,
        IValidator<CreateCategoryCommand> validator,
        ISettingsUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CreateCategoryResponse>> Handle(
        CreateCategoryCommand request,
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
                return ApplicationResult<CreateCategoryResponse>.Failure(errors, "Validation failed");
            }

            // Validate business rules
            var businessValidation = _validationService.ValidateCategoryCreation(
                request.Name, 
                request.Description, 
                request.SectionId, 
                request.DisplayOrder);

            if (!businessValidation.IsValid)
            {
                return ApplicationResult<CreateCategoryResponse>.Failure(
                    businessValidation.Errors, 
                    "Business validation failed");
            }

            // Check if section exists
            var section = await _settingsRepository.GetSectionByIdAsync(request.SectionId);
            if (section == null)
            {
                return ApplicationResult<CreateCategoryResponse>.Failure(
                    new List<string> { "Section not found" },
                    "Section does not exist");
            }

            // Check if category name is unique within the section
            var isNameUnique = await _settingsRepository.CategoryNameExistsAsync(request.Name, request.SectionId);
            if (isNameUnique)
            {
                return ApplicationResult<CreateCategoryResponse>.Failure(
                    new List<string> { "Category name already exists in this section" },
                    "Category name must be unique within the section");
            }

            // Create the category
            var category = new SettingsCategory(request.Name, request.Description, request.SectionId, request.DisplayOrder);
            await _settingsRepository.AddCategoryAsync(category);

            await _unitOfWork.CommitAsync(cancellationToken);

            // Return response
            var response = new CreateCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                SectionId = category.SectionId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            return ApplicationResult<CreateCategoryResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreateCategoryResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to create category");
        }
    }
}
