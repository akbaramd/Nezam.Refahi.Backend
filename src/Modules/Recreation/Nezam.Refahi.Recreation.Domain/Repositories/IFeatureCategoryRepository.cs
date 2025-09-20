using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for FeatureCategory entity operations
/// </summary>
public interface IFeatureCategoryRepository : IRepository<FeatureCategory, Guid>
{
  
}