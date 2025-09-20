namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwSeperationPlan
{
    public int ردیف { get; set; }

    public string شهرستان { get; set; } = null!;

    public DateTime تاریخثبت { get; set; }

    public string ناممالک { get; set; } = null!;

    public string نامخانوادگی { get; set; } = null!;

    public string? کدملی { get; set; }

    public string? موبایلمالک { get; set; }

    public string آدرس { get; set; } = null!;

    public string پلاکثبتی { get; set; } = null!;

    public int تعدادواحد { get; set; }

    public string? شمارهپروانه { get; set; }

    public DateTime? تاریخپروانه { get; set; }

    public double متراژ { get; set; }

    public int تعدادسقف { get; set; }

    public string وضعیتپرونده { get; set; } = null!;

    public double? متراژنقشهبردار { get; set; }

    public int? تعدادسقفنقشهبردار { get; set; }

    public bool? تجاری { get; set; }

    public string? شمارهعضویتمهندس { get; set; }

    public string? ناممهندس { get; set; }

    public string? نامخانوادگیمهندس { get; set; }

    public bool? سرگروه { get; set; }

    public string? وضعیتمهندس { get; set; }

    public DateTime? اولیننقشهبرداری { get; set; }

    public DateTime? آخریننقشهبرداری { get; set; }
}
