using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Accommodation;

public class GuestTests
{
    [Fact]
    public void Guest_Creation_With_Valid_Parameters_Creates_Guest()
    {
        // Arrange
        string firstName = "Mohammad";
        string lastName = "Ahmadi";
        string nationalId = "2741153671";
        int age = 30;

        // Act
        var guest = new Guest(firstName, lastName, nationalId, age);

        // Assert
        Assert.Equal(firstName, guest.FirstName);
        Assert.Equal(lastName, guest.LastName);
        Assert.Equal(new NationalId(nationalId).Value, guest.NationalId.Value);
        Assert.Equal(age, guest.Age);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Guest_Creation_With_Invalid_FirstName_Throws_Exception(string invalidFirstName)
    {
        // Arrange
        string lastName = "Ahmadi";
        string nationalId = "2741153671";
        int age = 30;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Guest(invalidFirstName, lastName, nationalId, age));
        Assert.Contains("first name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Guest_Creation_With_Invalid_LastName_Throws_Exception(string invalidLastName)
    {
        // Arrange
        string firstName = "Mohammad";
        string nationalId = "2741153671";
        int age = 30;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Guest(firstName, invalidLastName, nationalId, age));
        Assert.Contains("last name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void Guest_Creation_With_Invalid_Age_Throws_Exception(int invalidAge)
    {
        // Arrange
        string firstName = "Mohammad";
        string lastName = "Ahmadi";
        string nationalId = "2741153671";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Guest(firstName, lastName, nationalId, invalidAge));
        Assert.Contains("age", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Guest_Creation_With_Invalid_NationalId_Throws_Exception()
    {
        // Arrange
        string firstName = "Mohammad";
        string lastName = "Ahmadi";
        string invalidNationalId = "123"; // Assuming NationalId has validation logic requiring more digits
        int age = 30;

        // Act & Assert
        // The exception will come from the NationalId value object
        Assert.ThrowsAny<Exception>(() => new Guest(firstName, lastName, invalidNationalId, age));
    }

    [Fact]
    public void FullName_Returns_Correctly_Formatted_Name()
    {
        // Arrange
        string firstName = "Mohammad";
        string lastName = "Ahmadi";
        string nationalId = "2741153671";
        int age = 30;
        var guest = new Guest(firstName, lastName, nationalId, age);
        string expectedFullName = $"{firstName} {lastName}";

        // Act
        string actualFullName = guest.FullName;

        // Assert
        Assert.Equal(expectedFullName, actualFullName);
    }

    [Fact]
    public void UpdatePersonalInfo_Updates_Guest_Properties()
    {
        // Arrange
        var guest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        string newFirstName = "Ali";
        string newLastName = "Rezaei";
        int newAge = 35;

        // Act
        guest.UpdatePersonalInfo(newFirstName, newLastName, newAge);

        // Assert
        Assert.Equal(newFirstName, guest.FirstName);
        Assert.Equal(newLastName, guest.LastName);
        Assert.Equal(newAge, guest.Age);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdatePersonalInfo_With_Invalid_FirstName_Throws_Exception(string invalidFirstName)
    {
        // Arrange
        var guest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        string newLastName = "Rezaei";
        int newAge = 35;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            guest.UpdatePersonalInfo(invalidFirstName, newLastName, newAge));
        Assert.Contains("first name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdatePersonalInfo_With_Invalid_LastName_Throws_Exception(string invalidLastName)
    {
        // Arrange
        var guest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        string newFirstName = "Ali";
        int newAge = 35;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            guest.UpdatePersonalInfo(newFirstName, invalidLastName, newAge));
        Assert.Contains("last name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void UpdatePersonalInfo_With_Invalid_Age_Throws_Exception(int invalidAge)
    {
        // Arrange
        var guest = new Guest("Mohammad", "Ahmadi", "2741153671", 30);
        string newFirstName = "Ali";
        string newLastName = "Rezaei";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            guest.UpdatePersonalInfo(newFirstName, newLastName, invalidAge));
        Assert.Contains("age", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
