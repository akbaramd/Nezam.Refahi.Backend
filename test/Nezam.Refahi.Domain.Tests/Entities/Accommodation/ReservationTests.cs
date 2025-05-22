using System;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.Tests.TestHelpers;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class ReservationTests
{
    private readonly Hotel _hotel;
    private readonly Guest _primaryGuest;
    private readonly DateRange _stayPeriod;
    private readonly string _specialRequests;

    public ReservationTests()
    {
        // Set up common test objects
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province","Iran - Urmia");
        var pricePerNight = new Money(100.50m, "USD");
        _hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, pricePerNight, 100);
        _primaryGuest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        _stayPeriod = new DateRange(
            DateOnly.FromDateTime(DateTime.Today), 
            DateOnly.FromDateTime(DateTime.Today.AddDays(3)));
        _specialRequests = "Late check-in requested";
    }

    [Fact]
    public void Reservation_Creation_With_Valid_Parameters_Creates_Reservation()
    {
        // Act
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod, _specialRequests);

        // Assert
        Assert.Equal(_hotel.Id, reservation.HotelId);
        Assert.Equal(_primaryGuest.Id, reservation.PrimaryGuestId);
        Assert.Equal(_stayPeriod, reservation.StayPeriod);
        Assert.Equal(_specialRequests, reservation.SpecialRequests);
        Assert.Equal(ReservationStatus.InProgress, reservation.Status); // Default status for paid hotels
        Assert.Equal(_hotel.PricePerNight * _stayPeriod.NightCount, reservation.TotalPrice);
        Assert.Single(reservation.Guests); // Primary guest is automatically added
        Assert.Contains(_primaryGuest, reservation.Guests);
        Assert.False(reservation.IsLockExpired()); // Lock should be valid initially
    }

    [Fact]
    public void Reservation_Creation_With_Free_Hotel_Sets_Status_To_Confirmed()
    {
        // Arrange
        var freeHotel = new Hotel(Guid.NewGuid(),
            "Free Hotel", 
            "A free hotel", 
            new LocationReference(Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province","Iran - Urmia"),
            new Money(0m, "USD"), 
            100);

        // Act
        var reservation = new Reservation(freeHotel, _primaryGuest, _stayPeriod);

        // Assert
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.True(reservation.TotalPrice.IsFree);
    }

    [Fact]
    public void Reservation_Creation_With_Unavailable_Hotel_Throws_Exception()
    {
        // Arrange
        _hotel.SetAvailability(false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            new Reservation(_hotel, _primaryGuest, _stayPeriod));
    }

    [Fact]
    public void Reservation_Creation_With_Null_Hotel_Throws_Exception()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Reservation(null, _primaryGuest, _stayPeriod));
    }

    [Fact]
    public void Reservation_Creation_With_Null_PrimaryGuest_Throws_Exception()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Reservation(_hotel, null, _stayPeriod));
    }

    [Fact]
    public void Reservation_Creation_With_Null_StayPeriod_Throws_Exception()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Reservation(_hotel, _primaryGuest, null));
    }

    [Fact]
    public void AddGuest_Adds_Guest_To_Reservation()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var additionalGuest = new Guest("Ali", "Rezaei", "0741153671", 25);

        // Act
        reservation.AddGuest(additionalGuest);

        // Assert
        Assert.Equal(2, reservation.GuestCount);
        Assert.Contains(additionalGuest, reservation.Guests);
    }

    [Fact]
    public void AddGuest_With_Duplicate_NationalId_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var guestWithSameId = new Guest("Different", "Name", "2741153671", 40); // Same nationalId as primary guest

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.AddGuest(guestWithSameId));
    }

    [Fact]
    public void AddGuest_To_Non_InProgress_Reservation_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var additionalGuest = new Guest("Ali", "Rezaei", "2741153671", 25);
        reservation.MarkPaymentPending(); // Change status from InProgress

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.AddGuest(additionalGuest));
    }

    [Fact]
    public void RemoveGuest_Removes_Guest_From_Reservation()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var additionalGuest = new Guest("Ali", "Rezaei", "0741153671", 25);
        reservation.AddGuest(additionalGuest);
        Assert.Equal(2, reservation.GuestCount);

        // Act
        reservation.RemoveGuest(additionalGuest.Id);

        // Assert
        Assert.Equal(1, reservation.GuestCount);
        Assert.DoesNotContain(additionalGuest, reservation.Guests);
    }

    [Fact]
    public void RemoveGuest_Cannot_Remove_PrimaryGuest()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.RemoveGuest(_primaryGuest.Id));
    }

    [Fact]
    public void RemoveGuest_From_Non_InProgress_Reservation_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var additionalGuest = new Guest("Ali", "Rezaei", "0741153671", 25);
        reservation.AddGuest(additionalGuest);
        reservation.MarkPaymentPending(); // Change status from InProgress

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.RemoveGuest(additionalGuest.Id));
    }

    [Fact]
    public void UpdateSpecialRequests_Updates_Requests()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod, "Original request");
        string newRequests = "Updated special requests";

        // Act
        reservation.UpdateSpecialRequests(newRequests);

        // Assert
        Assert.Equal(newRequests, reservation.SpecialRequests);
    }

    [Fact]
    public void UpdateSpecialRequests_Can_Set_Null()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod, "Original request");

        // Act
        reservation.UpdateSpecialRequests(null);

        // Assert
        Assert.Null(reservation.SpecialRequests);
    }

    [Fact]
    public void UpdateSpecialRequests_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        reservation.MarkPaymentPending();
        reservation.ConfirmReservation(); // Now status is Confirmed

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            reservation.UpdateSpecialRequests("New requests"));
    }

    [Fact]
    public void MarkPaymentPending_Changes_Status()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        Assert.Equal(ReservationStatus.InProgress, reservation.Status);

        // Act
        reservation.MarkPaymentPending();

        // Assert
        Assert.Equal(ReservationStatus.PendingPayment, reservation.Status);
    }

    [Fact]
    public void MarkPaymentPending_For_Free_Reservation_Throws_Exception()
    {
        // Arrange
        var freeHotel = new Hotel(Guid.NewGuid(),
            "Free Hotel", 
            "A free hotel", 
            new LocationReference(Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province","Iran - Urmia"),
            new Money(0m, "USD"), 
            100);
        var reservation = new Reservation(freeHotel, _primaryGuest, _stayPeriod);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.MarkPaymentPending());
    }

    [Fact]
    public void MarkPaymentPending_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        reservation.MarkPaymentPending(); // Change to PendingPayment
        reservation.ConfirmReservation(); // Change to Confirmed

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.MarkPaymentPending());
    }

    [Fact]
    public void RecordPayment_Records_Payment_And_Confirms_Reservation()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        reservation.MarkPaymentPending();
        Guid paymentId = Guid.NewGuid();

        // Act
        reservation.RecordPayment(paymentId);

        // Assert
        Assert.Equal(paymentId, reservation.LastPaymentTransactionId);
        Assert.NotNull(reservation.LastPaymentDate);
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void RecordPayment_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        Guid paymentId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.RecordPayment(paymentId));
    }

    [Fact]
    public void RecordPayment_For_Free_Reservation_Throws_Exception()
    {
        // Arrange
        var freeHotel = new Hotel(Guid.NewGuid(),
            "Free Hotel", 
            "A free hotel", 
            new LocationReference(Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province","Iran - Urmia"),
            new Money(0m, "USD"), 
            100);
        var reservation = new Reservation(freeHotel, _primaryGuest, _stayPeriod);
        
        // Status will be Confirmed for free hotels, but let's force it to PendingPayment for the test
        typeof(Reservation).GetProperty("Status").SetValue(reservation, ReservationStatus.PendingPayment);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.RecordPayment(Guid.NewGuid()));
    }

    [Fact]
    public void ConfirmReservation_Changes_Status_To_Confirmed()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        reservation.MarkPaymentPending();

        // Act
        reservation.ConfirmReservation();

        // Assert
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void ConfirmReservation_For_Paid_Hotel_Without_PendingPayment_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Status is InProgress, not PendingPayment

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.ConfirmReservation());
    }

    [Fact]
    public void ConfirmReservation_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        reservation.MarkPaymentPending();
        reservation.ConfirmReservation(); // Now status is Confirmed
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.ConfirmReservation());
    }

    [Fact]
    public void CancelReservation_Changes_Status_To_Cancelled()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);

        // Act
        reservation.CancelReservation();

        // Assert
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void CancelReservation_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        typeof(Reservation).GetProperty("Status").SetValue(reservation, ReservationStatus.Completed);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.CancelReservation());
    }

    [Fact]
    public void CompleteReservation_Changes_Status_To_Completed()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Force reservation to Confirmed status
        typeof(Reservation).GetProperty("Status").SetValue(reservation, ReservationStatus.Confirmed);

        // Act
        reservation.CompleteReservation();

        // Assert
        Assert.Equal(ReservationStatus.Completed, reservation.Status);
    }

    [Fact]
    public void CompleteReservation_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Status is InProgress, not Confirmed

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.CompleteReservation());
    }

    [Fact]
    public void ExtendLock_Extends_Lock_Expiration_Time()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        var initialLockTime = reservation.LockExpirationTime;
        int extensionMinutes = 15;

        // Act
        reservation.ExtendLock(extensionMinutes);

        // Assert
        Assert.True(reservation.LockExpirationTime > initialLockTime);
    }

    [Fact]
    public void ExtendLock_With_Negative_Minutes_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => reservation.ExtendLock(-10));
    }

    [Fact]
    public void ExtendLock_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        typeof(Reservation).GetProperty("Status").SetValue(reservation, ReservationStatus.Confirmed);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.ExtendLock(15));
    }

    [Fact]
    public void IsLockExpired_Returns_True_When_Lock_Is_Expired()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Set lock to past time
        typeof(Reservation).GetProperty("LockExpirationTime").SetValue(
            reservation, DateTimeOffset.UtcNow.AddMinutes(-10));

        // Act & Assert
        Assert.True(reservation.IsLockExpired());
    }

    [Fact]
    public void IsLockExpired_Returns_False_When_Lock_Is_Not_Expired()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Lock is set to future time by constructor (default 30 minutes)

        // Act & Assert
        Assert.False(reservation.IsLockExpired());
    }

    [Fact]
    public void ExpireReservation_Changes_Status_To_Expired()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Set lock to past time
        typeof(Reservation).GetProperty("LockExpirationTime").SetValue(
            reservation, DateTimeOffset.UtcNow.AddMinutes(-10));

        // Act
        reservation.ExpireReservation();

        // Assert
        Assert.Equal(ReservationStatus.Expired, reservation.Status);
    }

    [Fact]
    public void ExpireReservation_For_Non_Expired_Lock_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Lock is not expired

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.ExpireReservation());
    }

    [Fact]
    public void ExpireReservation_In_Invalid_Status_Throws_Exception()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        // Set lock to past time
        typeof(Reservation).GetProperty("LockExpirationTime").SetValue(
            reservation, DateTimeOffset.UtcNow.AddMinutes(-10));
        // Change status to invalid state
        typeof(Reservation).GetProperty("Status").SetValue(reservation, ReservationStatus.Confirmed);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => reservation.ExpireReservation());
    }

    [Fact]
    public void GuestCount_Returns_Correct_Number_Of_Guests()
    {
        // Arrange
        var reservation = new Reservation(_hotel, _primaryGuest, _stayPeriod);
        Assert.Equal(1, reservation.GuestCount); // Primary guest only
        
        // Add more guests
        reservation.AddGuest(new Guest("Guest1", "Last1", "0741153671", 25));
        reservation.AddGuest(new Guest("Guest2", "Last2", "0841153671", 30));
        
        // Act & Assert
        Assert.Equal(3, reservation.GuestCount);
    }
}
