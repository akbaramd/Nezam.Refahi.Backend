using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.Tests.TestHelpers;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class HotelFeatureTests
{
    [Fact]
    public void HotelFeature_Creation_With_Valid_Parameters_Creates_Feature()
    {
        // Arrange
        string name = "WiFi";
        string value = "Available";
        string description = "Free high-speed WiFi";

        // Act
        var feature = new HotelFeature(name, value, description);

        // Assert
        Assert.Equal(name, feature.Name);
        Assert.Equal(value, feature.Value);
        Assert.Equal(description, feature.Description);
        Assert.Null(feature.Hotel);
        Assert.Equal(Guid.Empty, feature.HotelId);
    }

    [Fact]
    public void HotelFeature_Creation_With_No_Description_Creates_Feature_With_Null_Description()
    {
        // Arrange
        string name = "Parking";
        string value = "Available";

        // Act
        var feature = new HotelFeature(name, value);

        // Assert
        Assert.Equal(name, feature.Name);
        Assert.Equal(value, feature.Value);
        Assert.Null(feature.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void HotelFeature_Creation_With_Invalid_Name_Throws_Exception(string invalidName)
    {
        // Arrange
        string value = "Available";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new HotelFeature(invalidName, value));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void HotelFeature_Creation_With_Invalid_Value_Throws_Exception(string invalidValue)
    {
        // Arrange
        string name = "WiFi";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new HotelFeature(name, invalidValue));
        Assert.Contains("value", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetHotel_Establishes_Bidirectional_Relationship()
    {
        // Arrange
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province","Iran - Urmia");
        var money = Money.FromDecimal(100m, "USD");
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, money, 100);
        var feature = new HotelFeature("WiFi", "Available");

        // Act
        feature.SetHotel(hotel);

        // Assert
        Assert.Equal(hotel, feature.Hotel);
        Assert.Equal(hotel.Id, feature.HotelId);
    }

    [Fact]
    public void SetHotel_With_Null_Hotel_Throws_Exception()
    {
        // Arrange
        var feature = new HotelFeature("WiFi", "Available");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => feature.SetHotel(null));
    }

    [Fact]
    public void UpdateValue_Updates_Feature_Value()
    {
        // Arrange
        var feature = new HotelFeature("WiFi", "Available");
        string newValue = "Premium";

        // Act
        feature.UpdateValue(newValue);

        // Assert
        Assert.Equal(newValue, feature.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateValue_With_Invalid_Value_Throws_Exception(string invalidValue)
    {
        // Arrange
        var feature = new HotelFeature("WiFi", "Available");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            feature.UpdateValue(invalidValue));
        Assert.Contains("value", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateDescription_Updates_Feature_Description()
    {
        // Arrange
        var feature = new HotelFeature("WiFi", "Available", "Original description");
        string newDescription = "Updated description";

        // Act
        feature.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, feature.Description);
    }

    [Fact]
    public void UpdateDescription_With_Null_Sets_Description_To_Null()
    {
        // Arrange
        var feature = new HotelFeature("WiFi", "Available", "Original description");

        // Act
        feature.UpdateDescription(null);

        // Assert
        Assert.Null(feature.Description);
    }

    [Fact]
    public void BedroomCount_Factory_Method_Creates_Correct_Feature()
    {
        // Arrange
        int count = 3;

        // Act
        var feature = HotelFeature.BedroomCount(count);

        // Assert
        Assert.Equal("BedroomCount", feature.Name);
        Assert.Equal("3", feature.Value);
        Assert.Equal("تعداد اتاق خواب", feature.Description);
    }

    [Fact]
    public void HasWifi_Factory_Method_Creates_Correct_Feature()
    {
        // Act - true case
        var trueFeature = HotelFeature.HasWifi(true);
        
        // Assert - true case
        Assert.Equal("HasWifi", trueFeature.Name);
        Assert.Equal("True", trueFeature.Value);
        Assert.Equal("دسترسی به اینترنت", trueFeature.Description);
        
        // Act - false case
        var falseFeature = HotelFeature.HasWifi(false);
        
        // Assert - false case
        Assert.Equal("HasWifi", falseFeature.Name);
        Assert.Equal("False", falseFeature.Value);
        Assert.Equal("دسترسی به اینترنت", falseFeature.Description);
    }

    [Fact]
    public void HasBreakfast_Factory_Method_Creates_Correct_Feature()
    {
        // Act
        var feature = HotelFeature.HasBreakfast(true);

        // Assert
        Assert.Equal("HasBreakfast", feature.Name);
        Assert.Equal("True", feature.Value);
        Assert.Equal("صبحانه", feature.Description);
    }

    [Fact]
    public void HasParking_Factory_Method_Creates_Correct_Feature()
    {
        // Act
        var feature = HotelFeature.HasParking(true);

        // Assert
        Assert.Equal("HasParking", feature.Name);
        Assert.Equal("True", feature.Value);
        Assert.Equal("پارکینگ", feature.Description);
    }
}
