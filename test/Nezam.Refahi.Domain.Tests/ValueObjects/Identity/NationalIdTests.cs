using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.ValueObjects.Identity;

public class NationalIdTests
{
    [Fact]
    public void NationalId_Creation_With_Valid_Id_Creates_Instance()
    {
        // Arrange
        string validId = "0741153671"; // Valid Iranian National ID

        // Act
        var nationalId = new NationalId(validId);

        // Assert
        Assert.Equal(validId, nationalId.ToString());
    }

    [Fact]
    public void NationalId_Creation_With_Empty_Id_Throws_Exception()
    {
        // Arrange
        string emptyId = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new NationalId(emptyId));
        Assert.Contains("National ID cannot be empty", exception.Message);
    }

    [Fact]
    public void NationalId_Creation_With_Invalid_Length_Throws_Exception()
    {
        // Arrange
        string invalidLengthId = "12345"; // Less than 10 digits

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new NationalId(invalidLengthId));
        Assert.Contains("Invalid National ID format", exception.Message);
    }

    [Fact]
    public void NationalId_Creation_With_Non_Numeric_Throws_Exception()
    {
        // Arrange
        string nonNumericId = "123456789A"; // Contains a letter

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new NationalId(nonNumericId));
        Assert.Contains("Invalid National ID format", exception.Message);
    }

    [Fact]
    public void NationalId_Creation_With_All_Same_Digits_Throws_Exception()
    {
        // Arrange
        string allSameDigitsId = "1111111111"; // All digits are the same

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new NationalId(allSameDigitsId));
        Assert.Contains("Invalid National ID format", exception.Message);
    }

    [Theory]
    [InlineData("0741153671")] // Valid Iranian National ID
    [InlineData("0020361483")] // Another valid Iranian National ID
    public void NationalId_IsValid_Returns_True_For_Valid_Ids(string validId)
    {
        // Act & Assert
        Assert.True(NationalId.IsValid(validId));
    }

    [Theory]
    [InlineData("0000000000")] // All zeros
    [InlineData("1111111111")] // All same digit
    [InlineData("12345")] // Too short
    [InlineData("27411536711")] // Too long
    [InlineData("123456789A")] // Non-numeric
    public void NationalId_IsValid_Returns_False_For_Invalid_Ids(string invalidId)
    {
        // Act & Assert
        Assert.False(NationalId.IsValid(invalidId));
    }

    [Fact]
    public void NationalId_Equality_Works_Correctly()
    {
        // Arrange
        var id1 = new NationalId("0741153671");
        var id2 = new NationalId("0741153671");
        var id3 = new NationalId("2741153671");

        // Act & Assert
        Assert.Equal(id1, id2);
        Assert.NotEqual(id1, id3);
        Assert.True(id1 == id2);
        Assert.False(id1 == id3);
        Assert.False(id1 != id2);
        Assert.True(id1 != id3);
    }

    [Fact]
    public void NationalId_GetHashCode_Returns_Same_Value_For_Equal_Objects()
    {
        // Arrange
        var id1 = new NationalId("0741153671");
        var id2 = new NationalId("0741153671");

        // Act & Assert
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void NationalId_ImplicitConversion_To_String_Works_Correctly()
    {
        // Arrange
        string validId = "0741153671";
        var nationalId = new NationalId(validId);

        // Act
        string result = nationalId;

        // Assert
        Assert.Equal(validId, result);
    }
}
