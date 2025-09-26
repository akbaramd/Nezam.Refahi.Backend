using MCA.SharedKernel.Domain.Events;
using MediatR;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event published when a new tour is created
/// </summary>
public class TourCreatedEvent : DomainEvent
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public int AvailableSpots { get; set; }
    public List<string> Highlights { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public Guid CreatedBy { get; set; }
}
