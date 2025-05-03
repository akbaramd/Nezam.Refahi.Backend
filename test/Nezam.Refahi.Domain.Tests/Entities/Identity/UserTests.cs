using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Identity;

public class UserTests
{
  [Fact]
  public void User_Creation_Sets_Required_Properties()
  {
    // Arrange
    string firstName = "علی";
    string lastName = "محمدی";
    string nationalId = "2741153671";
    string phoneNumber = "09123456789";

    // Act
    var user = new User(firstName, lastName, nationalId, phoneNumber);

    // Assert
    Assert.Equal(firstName, user.FirstName);
    Assert.Equal(lastName, user.LastName);
    Assert.Equal(nationalId, user.NationalId.ToString());
    Assert.Equal(phoneNumber, user.PhoneNumber);
    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.NotEqual(default, user.CreatedAt);
    Assert.Null(user.ModifiedAt);
  }

  [Fact]
  public void User_Creation_With_Only_Required_Fields_Succeeds()
  {
    // Arrange
    string firstName = "علی";
    string lastName = "محمدی";
    string nationalId = "2741153671";

    // Act
    var user = new User(firstName, lastName, nationalId);

    // Assert
    Assert.Equal(firstName, user.FirstName);
    Assert.Equal(lastName, user.LastName);
    Assert.Equal(nationalId, user.NationalId.ToString());
    Assert.Null(user.PhoneNumber);
  }

  [Theory]
  [InlineData(null, "محمدی", "2741153671")]
  [InlineData("", "محمدی", "2741153671")]
  [InlineData("  ", "محمدی", "2741153671")]
  [InlineData("علی", null, "2741153671")]
  [InlineData("علی", "", "2741153671")]
  [InlineData("علی", "  ", "2741153671")]
  [InlineData("علی", "محمدی", null)]
  [InlineData("علی", "محمدی", "")]
  [InlineData("علی", "محمدی", "  ")]
  public void User_Creation_With_Missing_Required_Parameters_Throws_ArgumentException(
    string firstName,
    string lastName,
    string nationalId
  )
  {
    // Act & Assert
    Assert.Throws<ArgumentException>(() => new User(firstName, lastName, nationalId));
  }

  [Theory]
  [InlineData("علی", "محمدی", "123")]
  [InlineData("علی", "محمدی", "27411536711")]
  [InlineData("علی", "محمدی", "123abc4567")]
  public void User_Creation_With_Invalid_NationalId_Throws_ArgumentException(
    string firstName,
    string lastName,
    string nationalId
  )
  {
    // Act & Assert
    Assert.Throws<ArgumentException>(() => new User(firstName, lastName, nationalId));
  }

  [Fact]
  public void UpdateName_Updates_FirstName_LastName_And_ModifiedAt()
  {
    // Arrange
    var user = new User("علی", "محمدی", "2741153671");
    string newFirstName = "محمد";
    string newLastName = "احمدی";

    // Act
    user.UpdateName(newFirstName, newLastName);

    // Assert
    Assert.Equal(newFirstName, user.FirstName);
    Assert.Equal(newLastName, user.LastName);
    Assert.NotNull(user.ModifiedAt);
  }

  [Theory]
  [InlineData(null, "احمدی")]
  [InlineData("", "احمدی")]
  [InlineData("  ", "احمدی")]
  [InlineData("محمد", null)]
  [InlineData("محمد", "")]
  [InlineData("محمد", "  ")]
  public void UpdateName_With_Invalid_Parameters_Throws_ArgumentException(
    string newFirstName,
    string newLastName
  )
  {
    // Arrange
    var user = new User("علی", "محمدی", "2741153671");

    // Act & Assert
    Assert.Throws<ArgumentException>(() => user.UpdateName(newFirstName, newLastName));
  }

  [Fact]
  public void UpdatePhoneNumber_Updates_PhoneNumber_And_ModifiedAt()
  {
    // Arrange
    var user = new User("علی", "محمدی", "2741153671", phoneNumber: "09123456789");
    string newPhoneNumber = "09987654321";

    // Act
    user.UpdatePhoneNumber(newPhoneNumber);

    // Assert
    Assert.Equal(newPhoneNumber, user.PhoneNumber);
    Assert.NotNull(user.ModifiedAt);
  }

  [Fact]
  public void UpdatePhoneNumber_Can_Set_Null_PhoneNumber()
  {
    // Arrange
    var user = new User("علی", "محمدی", "2741153671", phoneNumber: "09123456789");

    // Act
    user.UpdatePhoneNumber(null);

    // Assert
    Assert.Null(user.PhoneNumber);
    Assert.NotNull(user.ModifiedAt);
  }
}
