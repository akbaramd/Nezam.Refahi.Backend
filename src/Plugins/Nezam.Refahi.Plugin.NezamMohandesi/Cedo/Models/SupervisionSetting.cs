namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionSetting
{
    public Guid Id { get; set; }

    public DateTime From { get; set; }

    public int EffectiveYears { get; set; }

    public double DesignAlpha { get; set; }

    public int DefaultCapacity { get; set; }

    public double OneFieldLegalWinnerPercentage { get; set; }

    public double TwoFieldLegalWinnerPercentage { get; set; }

    public double ThreeFieldLegalWinnerPercentage { get; set; }
}
