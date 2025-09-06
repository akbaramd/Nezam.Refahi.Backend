using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.CreateSection;

/// <summary>
/// Command to create a new settings section
/// </summary>
public record CreateSectionCommand : IRequest<ApplicationResult<CreateSectionResponse>>
{
    /// <summary>
    /// The name of the section
    /// </summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// The description of the section
    /// </summary>
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// The display order for sorting
    /// </summary>
    public int DisplayOrder { get; init; } = 0;
}
