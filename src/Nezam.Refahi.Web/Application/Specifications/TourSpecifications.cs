using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Web.Application.Specifications;

/// <summary>
/// Specification for active tours only
/// </summary>
public sealed class ActiveToursSpec : ISpecification<Tour>
{
    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.IsActive);
    }
}

/// <summary>
/// Specification for upcoming tours (starting after specified date)
/// </summary>
public sealed class UpcomingToursSpec : ISpecification<Tour>
{
    private readonly DateTime _fromDate;

    public UpcomingToursSpec(DateTime fromDate)
    {
        _fromDate = fromDate;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.IsActive && t.TourStart > _fromDate);
    }
}

/// <summary>
/// Specification for tours by status
/// </summary>
public sealed class ToursByStatusSpec : ISpecification<Tour>
{
    private readonly TourStatus _status;

    public ToursByStatusSpec(TourStatus status)
    {
        _status = status;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.Status == _status);
    }
}

/// <summary>
/// Specification for tours by date range
/// </summary>
public sealed class ToursByDateRangeSpec : ISpecification<Tour>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ToursByDateRangeSpec(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.TourStart >= _startDate && t.TourStart <= _endDate);
    }
}

/// <summary>
/// Specification for tours by price range
/// </summary>
public sealed class ToursByPriceRangeSpec : ISpecification<Tour>
{
    private readonly long _minPriceRials;
    private readonly long _maxPriceRials;

    public ToursByPriceRangeSpec(long minPriceRials, long maxPriceRials)
    {
        _minPriceRials = minPriceRials;
        _maxPriceRials = maxPriceRials;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => 
            t.Pricing.Any(p => 
                p.IsActive && 
                p.GetEffectivePrice().AmountRials >= _minPriceRials && 
                p.GetEffectivePrice().AmountRials <= _maxPriceRials));
    }
}

/// <summary>
/// Specification for tours by age requirements
/// </summary>
public sealed class ToursByAgeSpec : ISpecification<Tour>
{
    private readonly int _age;

    public ToursByAgeSpec(int age)
    {
        _age = age;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => 
            (!t.MinAge.HasValue || t.MinAge <= _age) &&
            (!t.MaxAge.HasValue || t.MaxAge >= _age));
    }
}

/// <summary>
/// Specification for tours with age restrictions
/// </summary>
public sealed class ToursWithAgeRestrictionsSpec : ISpecification<Tour>
{
    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.MinAge.HasValue || t.MaxAge.HasValue);
    }
}

/// <summary>
/// Specification for searching tours by title
/// </summary>
public sealed class ToursByTitleSearchSpec : ISpecification<Tour>
{
    private readonly string _searchTerm;

    public ToursByTitleSearchSpec(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
        
        _searchTerm = searchTerm.Trim().ToLower();
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => 
            t.Title.ToLower().Contains(_searchTerm) ||
            t.Description.ToLower().Contains(_searchTerm));
    }
}

/// <summary>
/// Specification for tours with photos
/// </summary>
public sealed class ToursWithPhotosSpec : ISpecification<Tour>
{
    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.Photos.Any());
    }
}

/// <summary>
/// Specification for tours with available spots
/// </summary>
public sealed class ToursWithAvailableSpotsSpec : ISpecification<Tour>
{
    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.GetAvailableSpots() > 0);
    }
}

/// <summary>
/// Specification for tours with registration open
/// </summary>
public sealed class ToursWithOpenRegistrationSpec : ISpecification<Tour>
{
    private readonly DateTime _currentDate;

    public ToursWithOpenRegistrationSpec(DateTime currentDate)
    {
        _currentDate = currentDate;
    }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        return query.Where(t => t.IsRegistrationOpen(_currentDate));
    }
}
