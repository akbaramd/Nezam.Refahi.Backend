using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class ParticipantMapper : IMapper<Participant, ParticipantDto>
{
    public Task<ParticipantDto> MapAsync(Participant source, CancellationToken cancellationToken = default)
    {
        var dto = new ParticipantDto
        {
            Id = source.Id,
            ReservationId = source.ReservationId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FullName = source.FullName,
            NationalNumber = source.NationalNumber,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            ParticipantType = source.ParticipantType.ToString(),
            BirthDate = source.BirthDate,
            EmergencyContactName = source.EmergencyContactName,
            EmergencyContactPhone = source.EmergencyContactPhone,
            Notes = source.Notes,
            RequiredAmountRials = source.RequiredAmount.AmountRials,
            PaidAmountRials = source.PaidAmount?.AmountRials,
            PaymentDate = source.PaymentDate,
            RegistrationDate = source.RegistrationDate,
            HasPaid = source.HasPaid,
            IsFullyPaid = source.IsFullyPaid,
            RemainingAmountRials = source.RemainingAmount.AmountRials,
            IsMainParticipant = source.IsMainParticipant,
            IsGuest = source.IsGuest
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(Participant source, ParticipantDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.ReservationId = source.ReservationId;
        destination.FirstName = source.FirstName;
        destination.LastName = source.LastName;
        destination.FullName = source.FullName;
        destination.NationalNumber = source.NationalNumber;
        destination.PhoneNumber = source.PhoneNumber;
        destination.Email = source.Email;
        destination.ParticipantType = source.ParticipantType.ToString();
        destination.BirthDate = source.BirthDate;
        destination.EmergencyContactName = source.EmergencyContactName;
        destination.EmergencyContactPhone = source.EmergencyContactPhone;
        destination.Notes = source.Notes;
        destination.RequiredAmountRials = source.RequiredAmount.AmountRials;
        destination.PaidAmountRials = source.PaidAmount?.AmountRials;
        destination.PaymentDate = source.PaymentDate;
        destination.RegistrationDate = source.RegistrationDate;
        destination.HasPaid = source.HasPaid;
        destination.IsFullyPaid = source.IsFullyPaid;
        destination.RemainingAmountRials = source.RemainingAmount.AmountRials;
        destination.IsMainParticipant = source.IsMainParticipant;
        destination.IsGuest = source.IsGuest;

        return Task.CompletedTask;
    }
}

