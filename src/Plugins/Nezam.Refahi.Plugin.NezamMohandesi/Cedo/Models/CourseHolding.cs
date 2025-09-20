namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseHolding
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid ExecutorId { get; set; }

    public int StepId { get; set; }

    public string? Title { get; set; }

    public DateTime? HoldingDate { get; set; }

    public DateTime RegStartDate { get; set; }

    public DateTime RegEndDate { get; set; }

    public string? Description { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public string? ApprovalNumber { get; set; }

    public DateTime? CertRegDate { get; set; }

    public string? CertRegNumber { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<CourseHoldingMember> CourseHoldingMembers { get; set; } = new List<CourseHoldingMember>();

    public virtual ICollection<CourseHoldingSignatoriese> CourseHoldingSignatorieses { get; set; } = new List<CourseHoldingSignatoriese>();

    public virtual CourseExecutor Executor { get; set; } = null!;

    public virtual ICollection<HoldingLesson> HoldingLessons { get; set; } = new List<HoldingLesson>();

    public virtual CourseHoldingStep Step { get; set; } = null!;
}
