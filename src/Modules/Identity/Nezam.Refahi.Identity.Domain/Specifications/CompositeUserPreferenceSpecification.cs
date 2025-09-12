using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Domain.Specifications;

/// <summary>
/// Composite specification for combining multiple specifications
/// </summary>
public class CompositeUserPreferenceSpecification
{
  private readonly List<Func<UserPreference, bool>> _specifications = new();

  public CompositeUserPreferenceSpecification Add(Func<UserPreference, bool> specification)
  {
    _specifications.Add(specification);
    return this;
  }

  public bool IsSatisfiedBy(UserPreference preference)
  {
    return _specifications.All(spec => spec(preference));
  }

  public IEnumerable<UserPreference> Filter(IEnumerable<UserPreference> preferences)
  {
    return preferences.Where(IsSatisfiedBy);
  }
}