using System;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.Tests.TestHelpers;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class HotelTests
{
    private readonly Guid _cityId = Guid.NewGuid();
    private readonly Guid _provinceId = Guid.NewGuid();
    private readonly string _cityName = "Tehran";
    private readonly string _provinceName = "Tehran Province";
    private readonly LocationReference _locationReference;
    private readonly Money _pricePerNight;

    public HotelTests()
    {
        _locationReference = new LocationReference(_cityId, _provinceId, _cityName, _provinceName,"Iran - Urmia");
        _pricePerNight = new Money(100.50m, "USD");
    }

    [Fact]
    public void Hotel_Creation_With_Valid_Parameters_Creates_Hotel()
    {
        // Arrange
        string name = "Grand Hotel";
        string description = "A luxury hotel in the heart of the city";
        int capacity = 100;

        // Act
        var hotel = new Hotel(Guid.NewGuid(),name, description, _locationReference, _pricePerNight, capacity);

        // Assert
        Assert.Equal(name, hotel.Name);
        Assert.Equal(description, hotel.Description);
        Assert.Equal(_locationReference, hotel.Location);
        Assert.Equal(_pricePerNight, hotel.PricePerNight);
        Assert.Equal(capacity, hotel.Capacity);
        Assert.True(hotel.IsAvailable);
        Assert.Empty(hotel.Features);
        Assert.Empty(hotel.Photos);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Hotel_Creation_With_Invalid_Name_Throws_Exception(string invalidName)
    {
        // Arrange
        string description = "A luxury hotel in the heart of the city";
        int capacity = 100;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Hotel(Guid.NewGuid(),invalidName, description, _locationReference, _pricePerNight, capacity));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Hotel_Creation_With_Invalid_Description_Throws_Exception(string invalidDescription)
    {
        // Arrange
        string name = "Grand Hotel";
        int capacity = 100;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Hotel(Guid.NewGuid(),name, invalidDescription, _locationReference, _pricePerNight, capacity));
        Assert.Contains("description", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Hotel_Creation_With_Invalid_Capacity_Throws_Exception(int invalidCapacity)
    {
        // Arrange
        string name = "Grand Hotel";
        string description = "A luxury hotel in the heart of the city";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Hotel(Guid.NewGuid(),name, description, _locationReference, _pricePerNight, invalidCapacity));
        Assert.Contains("capacity", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Hotel_Creation_With_Null_Location_Throws_Exception()
    {
        // Arrange
        string name = "Grand Hotel";
        string description = "A luxury hotel in the heart of the city";
        int capacity = 100;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Hotel(Guid.NewGuid(),name, description, null, _pricePerNight, capacity));
    }

    [Fact]
    public void Hotel_Creation_With_Null_PricePerNight_Throws_Exception()
    {
        // Arrange
        string name = "Grand Hotel";
        string description = "A luxury hotel in the heart of the city";
        int capacity = 100;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Hotel(Guid.NewGuid(),name, description, _locationReference, null, capacity));
    }

    [Fact]
    public void UpdateDetails_Updates_Hotel_Properties()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "Original description", _locationReference, _pricePerNight, 100);
        string newName = "Luxury Grand Hotel";
        string newDescription = "Updated luxury hotel description";
        var newPrice = new Money(150.75m, "USD");
        int newCapacity = 150;

        // Act
        hotel.UpdateDetails(newName, newDescription, newPrice, newCapacity);

        // Assert
        Assert.Equal(newName, hotel.Name);
        Assert.Equal(newDescription, hotel.Description);
        Assert.Equal(newPrice, hotel.PricePerNight);
        Assert.Equal(newCapacity, hotel.Capacity);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateDetails_With_Invalid_Name_Throws_Exception(string invalidName)
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "Original description", _locationReference, _pricePerNight, 100);
        string newDescription = "Updated luxury hotel description";
        var newPrice = new Money(150.75m, "USD");
        int newCapacity = 150;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            hotel.UpdateDetails(invalidName, newDescription, newPrice, newCapacity));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateDetails_With_Invalid_Description_Throws_Exception(string invalidDescription)
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "Original description", _locationReference, _pricePerNight, 100);
        string newName = "Luxury Grand Hotel";
        var newPrice = new Money(150.75m, "USD");
        int newCapacity = 150;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            hotel.UpdateDetails(newName, invalidDescription, newPrice, newCapacity));
        Assert.Contains("description", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateDetails_With_Invalid_Capacity_Throws_Exception(int invalidCapacity)
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "Original description", _locationReference, _pricePerNight, 100);
        string newName = "Luxury Grand Hotel";
        string newDescription = "Updated luxury hotel description";
        var newPrice = new Money(150.75m, "USD");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            hotel.UpdateDetails(newName, newDescription, newPrice, invalidCapacity));
        Assert.Contains("capacity", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateDetails_With_Null_PricePerNight_Throws_Exception()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "Original description", _locationReference, _pricePerNight, 100);
        string newName = "Luxury Grand Hotel";
        string newDescription = "Updated luxury hotel description";
        int newCapacity = 150;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            hotel.UpdateDetails(newName, newDescription, null, newCapacity));
    }

    [Fact]
    public void UpdateLocation_Updates_Hotel_Location()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var newLocationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Istanbul", "Istanbul Province","Iran - Urmia");

        // Act
        hotel.UpdateLocation(newLocationReference);

        // Assert
        Assert.Equal(newLocationReference, hotel.Location);
    }

    [Fact]
    public void UpdateLocation_With_Null_Location_Throws_Exception()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => hotel.UpdateLocation(null));
    }

    [Fact]
    public void AddFeature_Adds_Feature_To_Hotel()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var feature = new HotelFeature("WiFi", "Free high-speed WiFi");

        // Act
        hotel.AddFeature(feature);

        // Assert
        Assert.Single(hotel.Features);
        Assert.Contains(feature, hotel.Features);
        Assert.Equal(hotel, feature.Hotel);
    }

    [Fact]
    public void AddFeature_With_Null_Feature_Throws_Exception()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => hotel.AddFeature(null));
    }

    [Fact]
    public void AddFeature_With_Duplicate_Name_Throws_Exception()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var feature1 = new HotelFeature("WiFi", "Free high-speed WiFi");
        var feature2 = new HotelFeature("WiFi", "Different description but same name");
        hotel.AddFeature(feature1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => hotel.AddFeature(feature2));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void RemoveFeature_Removes_Feature_From_Hotel()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var feature = new HotelFeature("WiFi", "Free high-speed WiFi");
        hotel.AddFeature(feature);

        // Act
        hotel.RemoveFeature("WiFi");

        // Assert
        Assert.Empty(hotel.Features);
    }

    [Fact]
    public void RemoveFeature_With_Nonexistent_Name_Does_Nothing()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var feature = new HotelFeature("WiFi", "Free high-speed WiFi");
        hotel.AddFeature(feature);

        // Act
        hotel.RemoveFeature("Breakfast"); // This feature doesn't exist

        // Assert
        Assert.Single(hotel.Features); // The original feature should still be there
    }

    [Fact]
    public void SetAvailability_Updates_Hotel_Availability()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        Assert.True(hotel.IsAvailable); // Default is true

        // Act
        hotel.SetAvailability(false);

        // Assert
        Assert.False(hotel.IsAvailable);
        
        // Act again
        hotel.SetAvailability(true);
        
        // Assert again
        Assert.True(hotel.IsAvailable);
    }

    [Fact]
    public void CalculateTotalPrice_Returns_Correct_Total_For_Stay_Duration()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        var stayPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Today), 
                                      DateOnly.FromDateTime(DateTime.Today.AddDays(3))); // 3 nights
        var expectedTotal = new Money(_pricePerNight.Amount * 3, _pricePerNight.Currency);

        // Act
        var actualTotal = hotel.CalculateTotalPrice(stayPeriod);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
        Assert.Equal(301.50m, actualTotal.Amount); // 100.50 * 3 = 301.50
        Assert.Equal("USD", actualTotal.Currency);
    }

    [Fact]
    public void IsFree_Returns_True_When_PricePerNight_Is_Zero()
    {
        // Arrange
        var freePrice = new Money(0m, "USD");
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, freePrice, 100);

        // Act & Assert
        Assert.True(hotel.IsFree);
    }

    [Fact]
    public void IsFree_Returns_False_When_PricePerNight_Is_Not_Zero()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);

        // Act & Assert
        Assert.False(hotel.IsFree);
    }

    [Fact]
    public void IsAvailableForDates_Returns_False_When_Hotel_Is_Not_Available()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        hotel.SetAvailability(false);
        var dateRange = new DateRange(DateOnly.FromDateTime(DateTime.Today), 
                                      DateOnly.FromDateTime(DateTime.Today.AddDays(3)));

        // Act
        var result = hotel.IsAvailableForDates(dateRange);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAvailableForDates_Returns_True_When_Hotel_Is_Available()
    {
        // Arrange
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", _locationReference, _pricePerNight, 100);
        hotel.SetAvailability(true);
        var dateRange = new DateRange(DateOnly.FromDateTime(DateTime.Today), 
                                      DateOnly.FromDateTime(DateTime.Today.AddDays(3)));

        // Act
        var result = hotel.IsAvailableForDates(dateRange);

        // Assert
        Assert.True(result);
    }
}
