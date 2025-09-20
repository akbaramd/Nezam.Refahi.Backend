namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStep
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int Order { get; set; }

    public int CoordinatorPaymentPercent { get; set; }

    public int SupervisorPaymentPercent { get; set; }

    public int FormNumber { get; set; }

    public virtual ServiceField Field { get; set; } = null!;

    public virtual ICollection<SupervisionStepForm> SupervisionStepForms { get; set; } = new List<SupervisionStepForm>();

    public virtual ICollection<SupervisionStepItemGroup> SupervisionStepItemGroups { get; set; } = new List<SupervisionStepItemGroup>();

    public virtual ICollection<SupervisionStepPayment> SupervisionStepPayments { get; set; } = new List<SupervisionStepPayment>();
}
