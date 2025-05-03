using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.Tests.TestHelpers;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class HotelPhotoTests
{
    [Fact]
    public void HotelPhoto_Creation_With_Valid_Parameters_Creates_Photo()
    {
        // Arrange
        string url = "https://example.com/photos/hotel1.jpg";
        string caption = "Beautiful hotel view";
        string altText = "Hotel facade with mountains in background";
        bool isMainPhoto = true;

        // Act
        var photo = new HotelPhoto(url, caption, altText, isMainPhoto);

        // Assert
        Assert.Equal(url, photo.Url);
        Assert.Equal(caption, photo.Caption);
        Assert.Equal(altText, photo.AltText);
        Assert.Equal(isMainPhoto, photo.IsMainPhoto);
        Assert.Null(photo.Hotel);
        Assert.Equal(Guid.Empty, photo.HotelId);
    }

    [Fact]
    public void HotelPhoto_Creation_With_Only_Url_Creates_Photo_With_Default_Values()
    {
        // Arrange
        string url = "https://example.com/photos/hotel1.jpg";

        // Act
        var photo = new HotelPhoto(url);

        // Assert
        Assert.Equal(url, photo.Url);
        Assert.Null(photo.Caption);
        Assert.Null(photo.AltText);
        Assert.False(photo.IsMainPhoto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void HotelPhoto_Creation_With_Invalid_Url_Throws_Exception(string invalidUrl)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new HotelPhoto(invalidUrl));
        Assert.Contains("url", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetHotel_Establishes_Bidirectional_Relationship()
    {
        // Arrange
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province");
        var money = Money.FromDecimal(100m, "USD");
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, money, 100);
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg");

        // Act
        photo.SetHotel(hotel);

        // Assert
        Assert.Equal(hotel, photo.Hotel);
        Assert.Equal(hotel.Id, photo.HotelId);
    }

    [Fact]
    public void SetHotel_With_Null_Hotel_Throws_Exception()
    {
        // Arrange
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => photo.SetHotel(null));
    }

    [Fact]
    public void UpdateDetails_Updates_Photo_Properties()
    {
        // Arrange
        var photo = new HotelPhoto(
            "https://example.com/photos/old.jpg",
            "Old caption",
            "Old alt text"
        );
        string newUrl = "https://example.com/photos/new.jpg";
        string newCaption = "New caption";
        string newAltText = "New alt text";

        // Act
        photo.UpdateDetails(newUrl, newCaption, newAltText);

        // Assert
        Assert.Equal(newUrl, photo.Url);
        Assert.Equal(newCaption, photo.Caption);
        Assert.Equal(newAltText, photo.AltText);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateDetails_With_Invalid_Url_Throws_Exception(string invalidUrl)
    {
        // Arrange
        var photo = new HotelPhoto("https://example.com/photos/old.jpg");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            photo.UpdateDetails(invalidUrl, "New caption", "New alt text"));
        Assert.Contains("url", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateDetails_With_Null_Caption_And_AltText_Sets_Properties_To_Null()
    {
        // Arrange
        var photo = new HotelPhoto(
            "https://example.com/photos/old.jpg",
            "Old caption",
            "Old alt text"
        );
        string newUrl = "https://example.com/photos/new.jpg";

        // Act
        photo.UpdateDetails(newUrl, null, null);

        // Assert
        Assert.Equal(newUrl, photo.Url);
        Assert.Null(photo.Caption);
        Assert.Null(photo.AltText);
    }

    [Fact]
    public void SetAsMainPhoto_Sets_IsMainPhoto_To_True()
    {
        // Arrange
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg");
        Assert.False(photo.IsMainPhoto); // Default is false

        // Act
        photo.SetAsMainPhoto();

        // Assert
        Assert.True(photo.IsMainPhoto);
    }

    [Fact]
    public void UnsetAsMainPhoto_Sets_IsMainPhoto_To_False()
    {
        // Arrange
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg", isMainPhoto: true);
        Assert.True(photo.IsMainPhoto);

        // Act
        photo.UnsetAsMainPhoto();

        // Assert
        Assert.False(photo.IsMainPhoto);
    }

    [Fact]
    public void AddPhoto_To_Hotel_Establishes_Bidirectional_Relationship()
    {
        // Arrange
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province");
        var money = Money.FromDecimal(100m, "USD");
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, money, 100);
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg");

        // Act
        hotel.AddPhoto(photo);

        // Assert
        Assert.Contains(photo, hotel.Photos);
        Assert.Equal(hotel, photo.Hotel);
        Assert.Equal(hotel.Id, photo.HotelId);
    }

    [Fact]
    public void RemovePhoto_From_Hotel_Removes_Photo_From_Collection()
    {
        // Arrange
        var locationReference = new LocationReference(
            Guid.NewGuid(), Guid.NewGuid(), "Tehran", "Tehran Province");
        var money = Money.FromDecimal(100m, "USD");
        var hotel = new Hotel(Guid.NewGuid(),"Grand Hotel", "A luxury hotel", locationReference, money, 100);
        var photo = new HotelPhoto("https://example.com/photos/hotel1.jpg");
        hotel.AddPhoto(photo);
        
        // Verify photo was added
        Assert.Contains(photo, hotel.Photos);

        // Act
        hotel.RemovePhoto(photo.Id);

        // Assert
        Assert.Empty(hotel.Photos);
    }
}
