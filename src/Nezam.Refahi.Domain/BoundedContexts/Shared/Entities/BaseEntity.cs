namespace Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

/// <summary>
/// Base entity that all domain entities should inherit from
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? ModifiedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateModifiedAt()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}
