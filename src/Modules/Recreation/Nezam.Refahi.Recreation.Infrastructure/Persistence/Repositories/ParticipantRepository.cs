using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Participant entities
/// </summary>
public class ParticipantRepository : EfRepository<RecreationDbContext, Participant, Guid>, IParticipantRepository
{
    public ParticipantRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Participant>> GetByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.ReservationId == reservationId)
            .OrderBy(p => p.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Participant?> GetMainParticipantByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.ReservationId == reservationId &&
                                    p.ParticipantType == ParticipantType.Member, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetGuestParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.ReservationId == reservationId &&
                       p.ParticipantType == ParticipantType.Guest)
            .OrderBy(p => p.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetByReservationIdAndTypeAsync(Guid reservationId, ParticipantType participantType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.ReservationId == reservationId &&
                       p.ParticipantType == participantType)
            .OrderBy(p => p.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Participant?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.Email == email)
            .OrderBy(p => p.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetPaidParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.ReservationId == reservationId &&
                       p.PaidAmount != null &&
                       p.PaymentDate != null)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Participant>> GetUnpaidParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.ReservationId == reservationId &&
                       (p.PaidAmount == null || p.PaymentDate == null))
            .OrderBy(p => p.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNationalNumberRegisteredForReservationAsync(Guid reservationId, string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(p => p.ReservationId == reservationId &&
                          p.NationalNumber == nationalNumber, cancellationToken:cancellationToken);
    }

    public async Task<Participant?> GetByReservationIdAndNationalNumberAsync(Guid reservationId, string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.ReservationId == reservationId &&
                                    p.NationalNumber == nationalNumber, cancellationToken:cancellationToken);
    }

    public async Task<int> GetParticipantCountAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(p => p.ReservationId == reservationId, cancellationToken:cancellationToken);
    }

    protected override IQueryable<Participant> PrepareQuery(IQueryable<Participant> query)
    {
        return query;
    }
}