using FluentAssertions;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using System.Collections.ObjectModel;
using Xunit;

namespace Nezam.Refahi.Surveying.Tests.Domain.ValueObjects;

/// <summary>
/// Comprehensive tests for DemographySnapshot value object
/// Tests domain principles: immutability, validation, controlled data entry, equality
/// </summary>
public class DemographySnapshotTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateSnapshot()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            ["ProvinceCode"] = "TEH001",
            ["Gender"] = "Male"
        };

        // Act
        var snapshot = new DemographySnapshot(data);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.SchemaVersion.Should().Be(1);
        snapshot.Data.Should().BeEquivalentTo(data);
        snapshot.GetField("DisciplineCode").Should().Be("ENG001");
        snapshot.GetField("ProvinceCode").Should().Be("TEH001");
        snapshot.GetField("Gender").Should().Be("Male");
        snapshot.HasField("DisciplineCode").Should().BeTrue();
        snapshot.HasField("NonExistent").Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullData_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new DemographySnapshot(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    public void Constructor_WithInvalidKey_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["ValidKey"] = "value",
            ["InvalidKey"] = "value"
        };

        // Act & Assert
        var action = () => new DemographySnapshot(data);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not in the allowed demographic keys*");
    }

    [Fact]
    public void Constructor_WithEmptyData_ShouldCreateEmptySnapshot()
    {
        // Arrange
        var data = new Dictionary<string, string>();

        // Act
        var snapshot = new DemographySnapshot(data);

        // Assert
        snapshot.Data.Should().BeEmpty();
        snapshot.GetAllFields().Should().BeEmpty();
    }

    [Fact]
    public void Empty_ShouldCreateEmptySnapshot()
    {
        // Act
        var snapshot = DemographySnapshot.Empty();

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.SchemaVersion.Should().Be(1);
        snapshot.Data.Should().BeEmpty();
        snapshot.GetAllFields().Should().BeEmpty();
    }

    [Fact]
    public void WithField_WithValidKey_ShouldReturnNewSnapshotWithField()
    {
        // Arrange
        var originalSnapshot = DemographySnapshot.Empty();

        // Act
        var newSnapshot = originalSnapshot.WithField("DisciplineCode", "ENG001");

        // Assert
        newSnapshot.Should().NotBeSameAs(originalSnapshot);
        newSnapshot.GetField("DisciplineCode").Should().Be("ENG001");
        originalSnapshot.HasField("DisciplineCode").Should().BeFalse();
    }

    [Fact]
    public void WithField_WithInvalidKey_ShouldThrowArgumentException()
    {
        // Arrange
        var snapshot = DemographySnapshot.Empty();

        // Act & Assert
        var action = () => snapshot.WithField("InvalidKey", "value");
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not in the allowed demographic keys*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void WithField_WithEmptyKey_ShouldThrowArgumentException(string? key)
    {
        // Arrange
        var snapshot = DemographySnapshot.Empty();

        // Act & Assert
        var action = () => snapshot.WithField(key!, "value");
        action.Should().Throw<ArgumentException>()
            .WithMessage("Key cannot be empty*");
    }

    [Fact]
    public void WithField_WithExistingKey_ShouldUpdateValue()
    {
        // Arrange
        var snapshot = DemographySnapshot.Empty()
            .WithField("DisciplineCode", "ENG001");

        // Act
        var updatedSnapshot = snapshot.WithField("DisciplineCode", "ENG002");

        // Assert
        updatedSnapshot.GetField("DisciplineCode").Should().Be("ENG002");
        snapshot.GetField("DisciplineCode").Should().Be("ENG001"); // Original unchanged
    }

    [Fact]
    public void WithField_WithNullValue_ShouldStoreEmptyString()
    {
        // Arrange
        var snapshot = DemographySnapshot.Empty();

        // Act
        var newSnapshot = snapshot.WithField("DisciplineCode", null!);

        // Assert
        newSnapshot.GetField("DisciplineCode").Should().Be(string.Empty);
    }

    [Fact]
    public void GetAllFields_ShouldReturnReadOnlyDictionary()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            ["ProvinceCode"] = "TEH001"
        };
        var snapshot = new DemographySnapshot(data);

        // Act
        var fields = snapshot.GetAllFields();

        // Assert
        fields.Should().BeOfType<ReadOnlyDictionary<string, string>>();
        fields.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void Equality_WithSameData_ShouldBeEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            ["ProvinceCode"] = "TEH001"
        };
        var data2 = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            ["ProvinceCode"] = "TEH001"
        };

        var snapshot1 = new DemographySnapshot(data1);
        var snapshot2 = new DemographySnapshot(data2);

        // Act & Assert
        snapshot1.Should().Be(snapshot2);
        snapshot1.GetHashCode().Should().Be(snapshot2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentData_ShouldNotBeEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001"
        };
        var data2 = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG002"
        };

        var snapshot1 = new DemographySnapshot(data1);
        var snapshot2 = new DemographySnapshot(data2);

        // Act & Assert
        snapshot1.Should().NotBe(snapshot2);
    }

    [Fact]
    public void Equality_WithDifferentSchemaVersions_ShouldNotBeEqual()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001"
        };

        var snapshot1 = new DemographySnapshot(data, 1);
        var snapshot2 = new DemographySnapshot(data, 2);

        // Act & Assert
        snapshot1.Should().NotBe(snapshot2);
    }

    [Fact]
    public void Equality_WithDifferentKeyOrder_ShouldBeEqual()
    {
        // Arrange
        var data1 = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            ["ProvinceCode"] = "TEH001"
        };
        var data2 = new Dictionary<string, string>
        {
            ["ProvinceCode"] = "TEH001",
            ["DisciplineCode"] = "ENG001"
        };

        var snapshot1 = new DemographySnapshot(data1);
        var snapshot2 = new DemographySnapshot(data2);

        // Act & Assert
        snapshot1.Should().Be(snapshot2);
    }

    [Fact]
    public void AllowedKeys_ShouldContainExpectedKeys()
    {
        // Act & Assert
        DemographySnapshot.AllowedKeys.Should().Contain("DisciplineCode");
        DemographySnapshot.AllowedKeys.Should().Contain("ProvinceCode");
        DemographySnapshot.AllowedKeys.Should().Contain("LicenseGradeCode");
        DemographySnapshot.AllowedKeys.Should().Contain("SeniorityBand");
        DemographySnapshot.AllowedKeys.Should().Contain("EducationLevel");
        DemographySnapshot.AllowedKeys.Should().Contain("AgeGroup");
        DemographySnapshot.AllowedKeys.Should().Contain("Gender");
        DemographySnapshot.AllowedKeys.Should().Contain("OrganizationType");
        DemographySnapshot.AllowedKeys.Should().Contain("PositionLevel");
    }

    [Fact]
    public void Constructor_WithWhitespaceValues_ShouldCleanValues()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "  ENG001  ",
            ["ProvinceCode"] = null!
        };

        // Act
        var snapshot = new DemographySnapshot(data);

        // Assert
        snapshot.GetField("DisciplineCode").Should().Be("  ENG001  ");
        snapshot.GetField("ProvinceCode").Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithEmptyKey_ShouldSkipEmptyKey()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            ["DisciplineCode"] = "ENG001",
            [""] = "value",
            ["   "] = "value"
        };

        // Act
        var snapshot = new DemographySnapshot(data);

        // Assert
        snapshot.Data.Should().HaveCount(1);
        snapshot.Data.Should().ContainKey("DisciplineCode");
        snapshot.Data.Should().NotContainKey("");
        snapshot.Data.Should().NotContainKey("   ");
    }
}
