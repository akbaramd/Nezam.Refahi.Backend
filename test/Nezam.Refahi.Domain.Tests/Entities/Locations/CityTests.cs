using System;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Locations;

public class CityTests
{
    [Fact]
    public void City_Creation_Sets_Required_Properties()
    {
        // Arrange
        string name = "Tehran";
        string postalCode = "12345";

        // Act
        var city = new City(name, postalCode);

        // Assert
        Assert.Equal(name, city.Name);
        Assert.Equal(postalCode, city.PostalCode);
        Assert.NotEqual(Guid.Empty, city.Id);
        Assert.NotEqual(default, city.CreatedAt);
        Assert.Null(city.ModifiedAt);
        Assert.Equal(Guid.Empty, city.ProvinceId);
        Assert.Null(city.Province);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void City_Creation_With_Invalid_Name_Throws_Exception(string name)
    {
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => new City(name));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void City_Creation_With_Null_PostalCode_Is_Valid()
    {
        // Arrange
        string name = "Tehran";

        // Act
        var city = new City(name);

        // Assert
        Assert.Equal(name, city.Name);
        Assert.Null(city.PostalCode);
    }

    [Fact]
    public void SetProvince_Sets_Province_Relationship()
    {
        // Arrange
        var city = new City("Tehran");
        var province = new Province("Tehran Province", "THR");

        // Act
        city.SetProvince(province);

        // Assert
        Assert.Equal(province, city.Province);
        Assert.Equal(province.Id, city.ProvinceId);
        Assert.NotNull(city.ModifiedAt);
    }

    [Fact]
    public void SetProvince_With_Null_Province_Throws_Exception()
    {
        // Arrange
        var city = new City("Tehran");

        // Assert
        Assert.Throws<ArgumentNullException>(() => city.SetProvince(null));
    }

    [Fact]
    public void UpdateDetails_Updates_Properties_And_ModifiedAt()
    {
        // Arrange
        var city = new City("Tehran", "12345");
        var originalCreatedAt = city.CreatedAt;
        string newName = "New Tehran";
        string newPostalCode = "54321";

        // Act
        city.UpdateDetails(newName, newPostalCode);

        // Assert
        Assert.Equal(newName, city.Name);
        Assert.Equal(newPostalCode, city.PostalCode);
        Assert.NotNull(city.ModifiedAt);
        Assert.Equal(originalCreatedAt, city.CreatedAt);
    }

    [Fact]
    public void UpdateDetails_With_Null_PostalCode_Is_Valid()
    {
        // Arrange
        var city = new City("Tehran", "12345");
        string newName = "New Tehran";

        // Act
        city.UpdateDetails(newName, null);

        // Assert
        Assert.Equal(newName, city.Name);
        Assert.Null(city.PostalCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_With_Invalid_Name_Throws_Exception(string newName)
    {
        // Arrange
        var city = new City("Tehran");

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => city.UpdateDetails(newName));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToString_With_PostalCode_Returns_Expected_Format()
    {
        // Arrange
        string name = "Tehran";
        string postalCode = "12345";
        var city = new City(name, postalCode);
        string expected = $"{name} ({postalCode})";

        // Act
        var result = city.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_Without_PostalCode_Returns_Only_Name()
    {
        // Arrange
        string name = "Tehran";
        var city = new City(name);

        // Act
        var result = city.ToString();

        // Assert
        Assert.Equal(name, result);
    }

    [Fact]
    public void Bidirectional_Relationship_Works_Through_Province_AddCity()
    {
        // Arrange
        var province = new Province("Tehran Province", "THR");
        var city = new City("Tehran City");

        // Act
        province.AddCity(city);

        // Assert
        Assert.Equal(province, city.Province);
        Assert.Equal(province.Id, city.ProvinceId);
        Assert.Contains(city, province.Cities);
    }
}
