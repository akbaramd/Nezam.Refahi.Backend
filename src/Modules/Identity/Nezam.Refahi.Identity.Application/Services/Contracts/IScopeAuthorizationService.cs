using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Services.Contracts;

public interface IScopeAuthorizationService
{
    Task<ApplicationResult<bool>> ValidateScopeAsync(Guid userId, string requestedScope);
    Task<ApplicationResult<IEnumerable<string>>> GetUserScopesAsync(Guid userId);
    Task<ApplicationResult<bool>> HasPanelAccessAsync(Guid userId);
    Task<ApplicationResult<bool>> HasAppAccessAsync(Guid userId);
    Task<ApplicationResult<bool>> ValidateAnyScopeAsync(Guid userId, IEnumerable<string> requestedScopes);
    Task<ApplicationResult<bool>> ValidateAllScopesAsync(Guid userId, IEnumerable<string> requestedScopes);
    Task<ApplicationResult<IEnumerable<string>>> GetMissingScopesAsync(Guid userId, IEnumerable<string> requestedScopes);
    Task<ApplicationResult<bool>> HasAnyScopesAsync(Guid userId);
    Task<ApplicationResult<int>> GetScopeCountAsync(Guid userId);
}