using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Role aggregate root representing a domain role with employer and localized information
/// </summary>
public sealed class Role : Entity<Guid>
{
    public string Key { get; private set; } = string.Empty;           // Unique key like "Engineer", "Employee"
    public string Title { get; private set; } = string.Empty;  // English title
    public string? Description { get; private set; }                  // Role description
    public string? EmployerName { get; private set; }                 // Name of the employer/organization
    public string? EmployerCode { get; private set; }                 // Employer code/identifier
    public bool IsActive { get; private set; } = true;
    public int? SortOrder { get; private set; }                       // For display ordering

    // Navigation properties
    private readonly List<MemberRole> _memberRoles = new();
    public IReadOnlyCollection<MemberRole> MemberRoles => _memberRoles.AsReadOnly();

    // Private constructor for EF Core
    private Role() : base() { }

    /// <summary>
    /// Creates a new role
    /// </summary>
    public Role(string key, string title,  
        string? description = null, string? employerName = null, string? employerCode = null, int? sortOrder = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("English title cannot be empty", nameof(title));

        Key = key.Trim();
        Title = title.Trim();
        Description = description?.Trim();
        EmployerName = employerName?.Trim();
        EmployerCode = employerCode?.Trim();
        SortOrder = sortOrder;
        IsActive = true;
    }

    /// <summary>
    /// Updates the role information
    /// </summary>
    public void Update(string englishTitle, string persianTitle, 
        string? description = null, string? employerName = null, string? employerCode = null, int? sortOrder = null)
    {
        if (string.IsNullOrWhiteSpace(englishTitle))
            throw new ArgumentException("English title cannot be empty", nameof(englishTitle));
        if (string.IsNullOrWhiteSpace(persianTitle))
            throw new ArgumentException("Persian title cannot be empty", nameof(persianTitle));

        Title = englishTitle.Trim();
        Description = description?.Trim();
        EmployerName = employerName?.Trim();
        EmployerCode = employerCode?.Trim();
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Activates the role
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the role
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

  
}