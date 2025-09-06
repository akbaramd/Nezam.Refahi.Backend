using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.CreateCategory;

/// <summary>
/// Command to create a new settings category
/// </summary>
public record CreateCategoryCommand : IRequest<ApplicationResult<CreateCategoryResponse>>
{
    /// <summary>
    /// The name of the category
    /// </summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// The description of the category
    /// </summary>
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// The ID of the section this category belongs to
    /// </summary>
    public Guid SectionId { get; init; }
    
    /// <summary>
    /// The display order for sorting
    /// </summary>
    public int DisplayOrder { get; init; } = 0;
}
