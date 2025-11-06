using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class GuestParticipantMapper : IMapper<Participant, GuestParticipantDto>
{
    public Task<GuestParticipantDto> MapAsync(Participant source, CancellationToken cancellationToken = default)
    {
        var dto = new GuestParticipantDto
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            NationalNumber = source.NationalNumber,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            BirthDate = source.BirthDate,
            EmergencyContactName = source.EmergencyContactName,
            EmergencyContactPhone = source.EmergencyContactPhone,
            Notes = source.Notes
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(Participant source, GuestParticipantDto destination, CancellationToken cancellationToken = default)
    {
        // DTOs use init-only properties, so update mapping is not supported
        throw new NotImplementedException("Update mapping not supported for DTOs with init-only properties. Use MapAsync(Participant) instead.");
    }
}

