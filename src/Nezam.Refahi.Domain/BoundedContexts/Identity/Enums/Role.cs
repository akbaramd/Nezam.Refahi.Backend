using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Enums
{
  /// <summary>
  /// Represents the roles available in the Refahi system.
  /// Roles define sets of permissions for different user types.
  /// Flags are pure bit masks; composite checks should be done via extension methods.
  /// </summary>
  [Flags]
  public enum Role
  {
    /// <summary>No role assigned - no permissions.</summary>
    None = 0,

    /// <summary>Basic user role with minimal permissions: view public resources, update own profile.</summary>
    User = 1 << 0,

    /// <summary>Engineer role: submit engineering forms, access engineering dashboards, track project statuses.</summary>
    Engineer = 1 << 1,

    /// <summary>Editor role: create and edit content, approve or reject submissions.</summary>
    Editor = 1 << 2,

    /// <summary>Manager role: manage teams, assign tasks, view reports.</summary>
    Manager = 1 << 3,

    /// <summary>Administrator role: manage roles and permissions, configure system settings, full data access, audits.</summary>
    Administrator = 1 << 4,

    /// <summary>All roles combined.</summary>
    All = User | Engineer | Editor | Manager | Administrator
  }

  /// <summary>
  /// Extension methods for Role flags.
  /// </summary>
  public static class RoleExtensions
  {
    /// <summary>
    /// Returns true if the <paramref name="roles"/> includes all bits of <paramref name="required"/>.
    /// </summary>
    public static bool Includes(this Role roles, Role required)
      => (roles & required) == required;

    /// <summary>
    /// Returns true if the <paramref name="roles"/> includes any bit of <paramref name="check"/>.
    /// </summary>
    public static bool IncludesAny(this Role roles, Role check)
      => (roles & check) != 0;
  }
}
