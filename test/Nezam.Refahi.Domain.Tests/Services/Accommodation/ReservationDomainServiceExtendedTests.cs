using System;
using System.Threading.Tasks;
using Moq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Services;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Services.Accommodation;

public class ReservationDomainServiceExtendedTests
{
    private readonly Mock<IHotelRepository> _mockHotelRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly ReservationDomainService _service;
    private readonly Hotel _hotel;
    private readonly Guest _primaryGuest;
    private readonly Guest _additionalGuest;
    private readonly DateRange _dateRange;
    private readonly Reservation _reservation;

    public ReservationDomainServiceExtendedTests()
    {
        // Setup common test objects
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province");
        var pricePerNight = Money.FromDecimal(100.50m, "USD");
        _hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, pricePerNight, 100);
        _primaryGuest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        _additionalGuest = new Guest("Ali", "Rezaei", "0741153671", 25);
        _dateRange = new DateRange(DateOnly.FromDateTime(DateTime.Today), 
                                  DateOnly.FromDateTime(DateTime.Today.AddDays(3)));
        _reservation = new Reservation(_hotel, _primaryGuest, _dateRange, "Special request");
        
        // Add the additional guest to the reservation
        _reservation.AddGuest(_additionalGuest);

        // Setup mocks
        _mockHotelRepository = new Mock<IHotelRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        
        // Setup common mock behaviors
        _mockHotelRepository.Setup(r => r.GetByIdAsync(_hotel.Id)).ReturnsAsync(_hotel);
        _mockReservationRepository.Setup(r => r.GetByIdAsync(_reservation.Id)).ReturnsAsync(_reservation);
        
        // Create service
        _service = new ReservationDomainService(
            _mockHotelRepository.Object, 
            _mockReservationRepository.Object);
    }

    [Fact]
    public async Task RemoveGuestFromReservationAsync_Removes_Guest_From_Reservation()
    {
        // Arrange
        _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.RemoveGuestFromReservationAsync(_reservation.Id, _additionalGuest.Id);

        // Assert
        Assert.DoesNotContain(_reservation.Guests, g => g.Id == _additionalGuest.Id);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(_reservation.Id), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(_reservation), Times.Once);
    }

    [Fact]
    public async Task RemoveGuestFromReservationAsync_Throws_Exception_When_Reservation_Not_Found()
    {
        // Arrange
        var nonExistentReservationId = Guid.NewGuid();
        _mockReservationRepository.Setup(r => r.GetByIdAsync(nonExistentReservationId))
            .ReturnsAsync((Reservation)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveGuestFromReservationAsync(nonExistentReservationId, _additionalGuest.Id));
        
        Assert.Contains("not found", exception.Message);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(nonExistentReservationId), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task RemoveGuestFromReservationAsync_Throws_Exception_When_Removing_Primary_Guest()
    {
        // Arrange
        _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveGuestFromReservationAsync(_reservation.Id, _primaryGuest.Id));
        
        Assert.Contains("primary guest", exception.Message);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(_reservation.Id), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task RemoveGuestFromReservationAsync_Throws_Exception_When_Guest_Not_Found_In_Reservation()
    {
        // Arrange
        var nonExistentGuestId = Guid.NewGuid();
        _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveGuestFromReservationAsync(_reservation.Id, nonExistentGuestId));
        
        Assert.Contains("not found in reservation", exception.Message);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(_reservation.Id), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task UpdateSpecialRequestsAsync_Updates_Special_Requests()
    {
        // Arrange
        string newSpecialRequests = "Need extra pillows and late checkout";
        _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateSpecialRequestsAsync(_reservation.Id, newSpecialRequests);

        // Assert
        Assert.Equal(newSpecialRequests, _reservation.SpecialRequests);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(_reservation.Id), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(_reservation), Times.Once);
    }

    [Fact]
    public async Task ExtendReservationLockAsync_Extends_Lock_When_Not_Expired()
    {
        // Arrange
        var initialLockTime = _reservation.LockExpirationTime;
        int extensionMinutes = 15;
        
        _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ExtendReservationLockAsync(_reservation.Id, extensionMinutes);

        // Assert
        Assert.True(result);
        Assert.True(_reservation.LockExpirationTime > initialLockTime);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(_reservation.Id), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(_reservation), Times.Once);
    }

    [Fact]
    public async Task ExtendReservationLockAsync_Returns_False_When_Lock_Already_Expired()
    {
        // Arrange
        var reservationId = Guid.NewGuid();
        
        // Set up a custom implementation for GetByIdAsync that will return a reservation
        // where IsLockExpired() will return true
        _mockReservationRepository.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Guid id) => {
                // Create a real reservation
                var reservation = new Reservation(_hotel, _primaryGuest, _dateRange);
                
                // Use reflection to access the private backing field for testing purposes
                // This is a common technique for testing private state in unit tests
                var fieldInfo = typeof(Reservation).GetField("_lockExpirationTime", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (fieldInfo != null)
                {
                    // Set the lock expiration time to a past time
                    fieldInfo.SetValue(reservation, DateTimeOffset.UtcNow.AddMinutes(-30));
                }
                
                return reservation;
            });

        // Act
        var result = await _service.ExtendReservationLockAsync(reservationId, 15);

        // Assert
        Assert.False(result);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(reservationId), Times.Once);
        _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task ExtendReservationLockAsync_Throws_Exception_When_Minutes_Not_Positive()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ExtendReservationLockAsync(_reservation.Id, 0));
        
        Assert.Contains("must be greater than zero", exception.Message);
        _mockReservationRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
