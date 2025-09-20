namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ComplaintReviewType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ComplaintReview> ComplaintReviews { get; set; } = new List<ComplaintReview>();
}
