using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Domain.Entities;

/// <summary>
/// Represents a representative office/branch in the system
/// </summary>
public sealed class RepresentativeOffice : FullAggregateRoot<Guid>
{
    private RepresentativeOffice() { } // EF Core constructor

    public RepresentativeOffice(
        string code,
        string externalCode,
        string name,
        string address,
        string? managerName = null,
        string? managerPhone = null,
        DateTime? establishedDate = null):base(Guid.NewGuid())
    {
        Code = code;
        ExternalCode = externalCode;
        Name = name;
        Address = address;
        ManagerName = managerName;
        ManagerPhone = managerPhone;
        EstablishedDate = establishedDate;
        IsActive = true;
    }

    /// <summary>
    /// Unique code for the office (e.g., "URM001", "TBR002")
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// External code used in organizations (e.g., "ORG001", "BRANCH_001")
    /// </summary>
    public string ExternalCode { get; private set; } = string.Empty;

    /// <summary>
    /// Display name of the office
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Physical address of the office
    /// </summary>
    public string Address { get; private set; } = string.Empty;

    /// <summary>
    /// Name of the office manager
    /// </summary>
    public string? ManagerName { get; private set; }

    /// <summary>
    /// Phone number of the office manager
    /// </summary>
    public string? ManagerPhone { get; private set; }

    /// <summary>
    /// Whether the office is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Date when the office was established
    /// </summary>
    public DateTime? EstablishedDate { get; private set; }

    /// <summary>
    /// Update office information
    /// </summary>
    public void UpdateInfo(string name, string address, string? managerName = null, string? managerPhone = null)
    {
        Name = name;
        Address = address;
        ManagerName = managerName;
        ManagerPhone = managerPhone;
    }

    /// <summary>
    /// Update external code
    /// </summary>
    public void UpdateExternalCode(string externalCode)
    {
        ExternalCode = externalCode;
    }

    /// <summary>
    /// Activate the office
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivate the office
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}
