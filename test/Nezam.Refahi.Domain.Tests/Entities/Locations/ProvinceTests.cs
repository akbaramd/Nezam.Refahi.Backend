using System;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Locations;

public class ProvinceTests
{
    [Fact]
    public void Province_Creation_Sets_Required_Properties()
    {
        // Arrange
        string name = "Tehran";
        string code = "THR";

        // Act
        var province = new Province(name, code);

        // Assert
        Assert.Equal(name, province.Name);
        Assert.Equal(code, province.Code);
        Assert.NotEqual(Guid.Empty, province.Id);
        Assert.NotEqual(default, province.CreatedAt);
        Assert.Null(province.ModifiedAt);
        Assert.Empty(province.Cities);
    }

    [Theory]
    [InlineData(null, "THR")]
    [InlineData("", "THR")]
    [InlineData("   ", "THR")]
    public void Province_Creation_With_Invalid_Name_Throws_Exception(string name, string code)
    {
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => new Province(name, code));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Tehran", null)]
    [InlineData("Tehran", "")]
    [InlineData("Tehran", "   ")]
    public void Province_Creation_With_Invalid_Code_Throws_Exception(string name, string code)
    {
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => new Province(name, code));
        Assert.Contains("code", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateName_Updates_Name_And_ModifiedAt()
    {
        // Arrange
        var province = new Province("Tehran", "THR");
        var originalCreatedAt = province.CreatedAt;
        string newName = "New Tehran";

        // Act
        province.UpdateName(newName);

        // Assert
        Assert.Equal(newName, province.Name);
        Assert.NotNull(province.ModifiedAt);
        Assert.Equal(originalCreatedAt, province.CreatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_With_Invalid_Name_Throws_Exception(string newName)
    {
        // Arrange
        var province = new Province("Tehran", "THR");

        // Assert
        var exception = Assert.Throws<ArgumentException>(() => province.UpdateName(newName));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddCity_Adds_City_To_Collection()
    {
        // Arrange
        var province = new Province("Tehran", "THR");
        var city = new City("Tehran City");

        // Act
        province.AddCity(city);

        // Assert
        Assert.Single(province.Cities);
        Assert.Equal(city, province.Cities.First());
        Assert.Equal(province.Id, city.ProvinceId);
        Assert.Equal(province, city.Province);
        Assert.NotNull(province.ModifiedAt);
    }

    [Fact]
    public void AddCity_With_Null_City_Throws_Exception()
    {
        // Arrange
        var province = new Province("Tehran", "THR");

        // Assert
        Assert.Throws<ArgumentNullException>(() => province.AddCity(null));
    }

    [Fact]
    public void AddCity_With_Duplicate_Name_Throws_Exception()
    {
        // Arrange
        var province = new Province("Tehran", "THR");
        var city1 = new City("Tehran City");
        var city2 = new City("Tehran City");
        province.AddCity(city1);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => province.AddCity(city2));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void ToString_Returns_Expected_Format()
    {
        // Arrange
        string name = "Tehran";
        string code = "THR";
        var province = new Province(name, code);
        string expected = $"{name} ({code})";

        // Act
        var result = province.ToString();

        // Assert
        Assert.Equal(expected, result);
    }
}
