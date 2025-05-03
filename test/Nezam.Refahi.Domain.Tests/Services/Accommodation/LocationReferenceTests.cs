using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class LocationReferenceTests
{
  [Fact]
  public void LocationReference_Creation_Sets_Required_Properties()
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    string cityName = "Tehran";
    string provinceName = "Tehran Province";

    // Act
    var locationRef = new LocationReference(cityId, provinceId, cityName, provinceName);

    // Assert
    Assert.Equal(cityId, locationRef.CityId);
    Assert.Equal(provinceId, locationRef.ProvinceId);
    Assert.Equal(cityName, locationRef.CityName);
    Assert.Equal(provinceName, locationRef.ProvinceName);
  }

  [Fact]
  public void LocationReference_Creation_With_Empty_CityId_Throws_Exception()
  {
    // Arrange
    Guid cityId = Guid.Empty;
    Guid provinceId = Guid.NewGuid();
    string cityName = "Tehran";
    string provinceName = "Tehran Province";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(
      () => new LocationReference(cityId, provinceId, cityName, provinceName)
    );
    Assert.Contains("city", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void LocationReference_Creation_With_Empty_ProvinceId_Throws_Exception()
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.Empty;
    string cityName = "Tehran";
    string provinceName = "Tehran Province";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(
      () => new LocationReference(cityId, provinceId, cityName, provinceName)
    );
    Assert.Contains("province", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void LocationReference_Creation_With_Invalid_CityName_Throws_Exception(string cityName)
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    string provinceName = "Tehran Province";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(
      () => new LocationReference(cityId, provinceId, cityName, provinceName)
    );
    Assert.Contains("city name", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void LocationReference_Creation_With_Invalid_ProvinceName_Throws_Exception(
    string provinceName
  )
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    string cityName = "Tehran";

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(
      () => new LocationReference(cityId, provinceId, cityName, provinceName)
    );
    Assert.Contains("province name", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void ToString_Returns_Expected_Format()
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    string cityName = "Tehran";
    string provinceName = "Tehran Province";
    var locationRef = new LocationReference(cityId, provinceId, cityName, provinceName);
    string expected = $"{cityName}, {provinceName}";

    // Act
    var result = locationRef.ToString();

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void Equals_With_Same_Values_Returns_True()
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    var location1 = new LocationReference(cityId, provinceId, "Tehran", "Tehran Province");
    var location2 = new LocationReference(cityId, provinceId, "Tehran City", "Tehran State");

    // Act & Assert
    Assert.True(location1.Equals(location2));
    Assert.Equal(location1, location2);
    Assert.True(location1 == location2);
    Assert.False(location1 != location2);
  }

  [Fact]
  public void Equals_With_Different_Values_Returns_False()
  {
    // Arrange
    var location1 = new LocationReference(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Tehran",
      "Tehran Province"
    );
    var location2 = new LocationReference(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Tehran",
      "Tehran Province"
    );

    // Act & Assert
    Assert.False(location1.Equals(location2));
    Assert.NotEqual(location1, location2);
    Assert.False(location1 == location2);
    Assert.True(location1 != location2);
  }

  [Fact]
  public void GetHashCode_With_Same_Values_Returns_Same_HashCode()
  {
    // Arrange
    Guid cityId = Guid.NewGuid();
    Guid provinceId = Guid.NewGuid();
    var location1 = new LocationReference(cityId, provinceId, "Tehran", "Tehran Province");
    var location2 = new LocationReference(cityId, provinceId, "Tehran City", "Tehran State");

    // Act & Assert
    Assert.Equal(location1.GetHashCode(), location2.GetHashCode());
  }
}
