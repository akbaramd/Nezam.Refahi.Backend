using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Guest participant information DTO
/// </summary>
public class GuestParticipantDto : IStaticMapper<Participant, GuestParticipantDto>
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string NationalNumber { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string? Email { get; init; }
    public ParticipantType ParticipantType { get; init; } = ParticipantType.Guest;
    public DateTime BirthDate { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactPhone { get; init; }
    public string? Notes { get; init; }

    /// <summary>
    /// Maps from Participant entity to GuestParticipantDto
    /// </summary>
    /// <param name="entity">The source participant entity</param>
    /// <returns>Mapped GuestParticipantDto</returns>
    public static GuestParticipantDto MapFrom(Participant entity)
    {
        return new GuestParticipantDto
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NationalNumber = entity.NationalNumber,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            ParticipantType = entity.ParticipantType,
            BirthDate = entity.BirthDate,
            EmergencyContactName = entity.EmergencyContactName,
            EmergencyContactPhone = entity.EmergencyContactPhone,
            Notes = entity.Notes
        };
    }

    /// <summary>
    /// Maps from GuestParticipantDto to Participant entity
    /// Note: This operation is not supported as DTOs should not create entities
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>Not supported</returns>
    public static Participant MapTo(GuestParticipantDto dto)
    {
        throw new NotSupportedException("Mapping from GuestParticipantDto to Participant is not supported. Use domain services to create entities.");
    }
}