using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourPhoto entities
/// </summary>
public class TourPhotoRepository : EfRepository<RecreationDbContext, TourPhoto, Guid>, ITourPhotoRepository
{
    public TourPhotoRepository(RecreationDbContext context) : base(context)
    {
    }

    protected override IQueryable<TourPhoto> PrepareQuery(IQueryable<TourPhoto> query)
    {
        return query;
    }
}