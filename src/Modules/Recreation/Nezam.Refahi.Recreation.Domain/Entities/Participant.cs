using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Entity representing a participant in a tour reservation
/// </summary>
public sealed class Participant : Entity<Guid>
{
    public Guid ReservationId { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string NationalNumber { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public ParticipantType ParticipantType { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public string? Notes { get; private set; }
    public Money RequiredAmount { get; private set; } = null!;
    public Money? PaidAmount { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    // Navigation property
    public TourReservation Reservation { get; private set; } = null!;

    // Private constructor for EF Core
    private Participant() : base() { }

    /// <summary>
    /// Creates a new participant
    /// </summary>
    public Participant(
        Guid reservationId,
        string firstName,
        string lastName,
        string nationalNumber,
        string phoneNumber,
            DateTime birthDate,
        ParticipantType participantType = ParticipantType.Guest,
        Money requiredAmount = null!,
        string? email = null,
    
        string? emergencyContactName = null,
        string? emergencyContactPhone = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation ID cannot be empty", nameof(reservationId));

        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidateNationalNumber(nationalNumber);
        ValidatePhoneNumber(phoneNumber);
        ValidateEmail(email);
        ValidateBirthDate(birthDate);

        if (requiredAmount == null)
            throw new ArgumentNullException(nameof(requiredAmount));

        ReservationId = reservationId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        NationalNumber = nationalNumber.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email?.Trim();
        ParticipantType = participantType;
        BirthDate = birthDate;
        EmergencyContactName = emergencyContactName?.Trim();
        EmergencyContactPhone = emergencyContactPhone?.Trim();
        Notes = notes?.Trim();
        RequiredAmount = requiredAmount;
        RegistrationDate = DateTime.UtcNow;
        PaidAmount = null;
        PaymentDate = null;
    }

    /// <summary>
    /// Gets participant's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Updates participant's personal information
    /// </summary>
    public void UpdatePersonalInfo(
        string firstName,
        string lastName,
        string nationalNumber,
        DateTime birthDate,
        string phoneNumber,
        string? email = null)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidateNationalNumber(nationalNumber);
        ValidatePhoneNumber(phoneNumber);
        ValidateEmail(email);
        ValidateBirthDate(birthDate);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        NationalNumber = nationalNumber.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email?.Trim();
        BirthDate = birthDate;
    }

    /// <summary>
    /// Updates emergency contact information
    /// </summary>
    public void UpdateEmergencyContact(string? emergencyContactName, string? emergencyContactPhone)
    {
        if (!string.IsNullOrWhiteSpace(emergencyContactPhone))
            ValidatePhoneNumber(emergencyContactPhone);

        EmergencyContactName = emergencyContactName?.Trim();
        EmergencyContactPhone = emergencyContactPhone?.Trim();
    }

    /// <summary>
    /// Sets participant type
    /// </summary>
    public void SetParticipantType(ParticipantType participantType)
    {
        ParticipantType = participantType;
    }

    /// <summary>
    /// Updates participant notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Checks if participant is main participant
    /// </summary>
    public bool IsMainParticipant => ParticipantType == ParticipantType.Member;

    /// <summary>
    /// Checks if participant is guest
    /// </summary>
    public bool IsGuest => ParticipantType == ParticipantType.Guest;

    /// <summary>
    /// Records payment information
    /// </summary>
    public void RecordPayment(Money amount)
    {
        PaidAmount = amount ?? throw new ArgumentNullException(nameof(amount));
        PaymentDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears payment information
    /// </summary>
    public void ClearPayment()
    {
        PaidAmount = null;
        PaymentDate = null;
    }

    /// <summary>
    /// Checks if payment has been made
    /// </summary>
    public bool HasPaid => PaidAmount != null && PaymentDate != null;

    /// <summary>
    /// Gets the remaining amount to be paid
    /// </summary>
    public Money RemainingAmount => RequiredAmount.Subtract(PaidAmount ?? Money.Zero);

    /// <summary>
    /// Checks if participant has fully paid
    /// </summary>
    public bool IsFullyPaid => PaidAmount != null && PaidAmount.AmountRials >= RequiredAmount.AmountRials;

    /// <summary>
    /// Updates the required amount (e.g., when pricing changes)
    /// </summary>
    public void UpdateRequiredAmount(Money amount)
    {
        RequiredAmount = amount ?? throw new ArgumentNullException(nameof(amount));
    }

    // Private validation methods
    private static void ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
        if (name.Length > 100)
            throw new ArgumentException($"{paramName} cannot exceed 100 characters", paramName);
    }

    private static void ValidateNationalNumber(string nationalNumber)
    {
        if (string.IsNullOrWhiteSpace(nationalNumber))
            throw new ArgumentException("National number cannot be empty", nameof(nationalNumber));
        if (nationalNumber.Length > 20)
            throw new ArgumentException("National number cannot exceed 20 characters", nameof(nationalNumber));
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
        if (phoneNumber.Length > 15)
            throw new ArgumentException("Phone number cannot exceed 15 characters", nameof(phoneNumber));
        // Basic phone number format validation (digits, +, -, spaces, parentheses)
        if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[\d\+\-\s\(\)]+$"))
            throw new ArgumentException("Phone number contains invalid characters", nameof(phoneNumber));
    }

    private static void ValidateEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (email.Length > 256)
                throw new ArgumentException("Email cannot exceed 256 characters", nameof(email));
            // Basic email format validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format", nameof(email));
        }
    }

    private static void ValidateBirthDate(DateTime? birthDate)
    {
        if (birthDate.HasValue)
        {
            if (birthDate.Value > DateTime.Today)
                throw new ArgumentException("Birth date cannot be in the future", nameof(birthDate));
            if (birthDate.Value < DateTime.Today.AddYears(-150))
                throw new ArgumentException("Birth date cannot be more than 150 years ago", nameof(birthDate));
        }
    }
}