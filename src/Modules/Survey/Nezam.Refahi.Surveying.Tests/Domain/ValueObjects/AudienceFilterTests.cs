using FluentAssertions;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using System.Text.Json;
using Xunit;

namespace Nezam.Refahi.Surveying.Tests.Domain.ValueObjects;

/// <summary>
/// Comprehensive tests for AudienceFilter value object
/// Tests domain principles: immutability, validation, DSL parsing, equality
/// </summary>
public class AudienceFilterTests
{
    [Fact]
    public void Constructor_WithValidExpression_ShouldCreateFilter()
    {
        // Arrange
        var filterExpression = """{"requiredFeatures": ["feature1", "feature2"]}""";

        // Act
        var filter = new AudienceFilter(filterExpression);

        // Assert
        filter.Should().NotBeNull();
        filter.FilterExpression.Should().Be(filterExpression);
        filter.FilterVersion.Should().Be(1);
        filter.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithValidExpressionAndVersion_ShouldCreateFilter()
    {
        // Arrange
        var filterExpression = """{"requiredFeatures": ["feature1"]}""";
        var version = 2;

        // Act
        var filter = new AudienceFilter(filterExpression, version);

        // Assert
        filter.Should().NotBeNull();
        filter.FilterExpression.Should().Be(filterExpression);
        filter.FilterVersion.Should().Be(version);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyExpression_ShouldThrowArgumentException(string? filterExpression)
    {
        // Act & Assert
        var action = () => new AudienceFilter(filterExpression!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Filter expression cannot be empty*");
    }

    [Fact]
    public void FromDefinitionCodes_WithValidCodes_ShouldCreateFilter()
    {
        // Arrange
        var requiredFeatures = new List<string> { "feature1", "feature2" };
        var requiredCapabilities = new List<string> { "capability1" };
        var excludedFeatures = new List<string> { "excluded1" };
        var excludedCapabilities = new List<string> { "excluded2" };

        // Act
        var filter = AudienceFilter.FromDefinitionCodes(
            requiredFeatures, 
            requiredCapabilities, 
            excludedFeatures, 
            excludedCapabilities);

        // Assert
        filter.Should().NotBeNull();
        filter.FilterExpression.Should().NotBeNullOrEmpty();
        filter.GetRequiredFeatureCodes().Should().BeEquivalentTo(requiredFeatures);
        filter.GetRequiredCapabilityCodes().Should().BeEquivalentTo(requiredCapabilities);
    }

    [Fact]
    public void FromDefinitionCodes_WithNullLists_ShouldCreateFilterWithEmptyLists()
    {
        // Act
        var filter = AudienceFilter.FromDefinitionCodes(null!, null!, null!, null!);

        // Assert
        filter.Should().NotBeNull();
        filter.GetRequiredFeatureCodes().Should().BeEmpty();
        filter.GetRequiredCapabilityCodes().Should().BeEmpty();
    }

    [Fact]
    public void ForMemberGroups_WithValidGroups_ShouldCreateFilter()
    {
        // Arrange
        var memberGroups = new List<string> { "group1", "group2" };

        // Act
        var filter = AudienceFilter.ForMemberGroups(memberGroups);

        // Assert
        filter.Should().NotBeNull();
        filter.FilterExpression.Should().NotBeNullOrEmpty();
        
        var criteria = filter.GetCriteria();
        criteria.Should().ContainKey("memberGroups");
    }

    [Fact]
    public void ForMemberGroups_WithNullList_ShouldCreateFilterWithEmptyList()
    {
        // Act
        var filter = AudienceFilter.ForMemberGroups(null!);

        // Assert
        filter.Should().NotBeNull();
        var criteria = filter.GetCriteria();
        criteria.Should().ContainKey("memberGroups");
    }

    [Fact]
    public void GetCriteria_WithValidJson_ShouldReturnParsedCriteria()
    {
        // Arrange
        var expectedCriteria = new Dictionary<string, object>
        {
            ["requiredFeatures"] = new List<string> { "feature1", "feature2" },
            ["requiredCapabilities"] = new List<string> { "capability1" }
        };
        var filterExpression = JsonSerializer.Serialize(expectedCriteria);
        var filter = new AudienceFilter(filterExpression);

        // Act
        var criteria = filter.GetCriteria();

        // Assert
        criteria.Should().NotBeNull();
        criteria.Should().ContainKey("requiredFeatures");
        criteria.Should().ContainKey("requiredCapabilities");
    }

    [Fact]
    public void GetCriteria_WithInvalidJson_ShouldReturnEmptyCriteria()
    {
        // Arrange
        var invalidJson = "invalid json";
        var filter = new AudienceFilter(invalidJson);

        // Act
        var criteria = filter.GetCriteria();

        // Assert
        criteria.Should().NotBeNull();
        criteria.Should().BeEmpty();
    }

    [Fact]
    public void GetRequiredFeatureCodes_WithValidFilter_ShouldReturnFeatureCodes()
    {
        // Arrange
        var requiredFeatures = new List<string> { "feature1", "feature2", "feature3" };
        var filter = AudienceFilter.FromDefinitionCodes(requiredFeatures, new List<string>());

        // Act
        var featureCodes = filter.GetRequiredFeatureCodes();

        // Assert
        featureCodes.Should().BeEquivalentTo(requiredFeatures);
    }

    [Fact]
    public void GetRequiredFeatureCodes_WithEmptyFilter_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = AudienceFilter.FromDefinitionCodes(new List<string>(), new List<string>());

        // Act
        var featureCodes = filter.GetRequiredFeatureCodes();

        // Assert
        featureCodes.Should().BeEmpty();
    }

    [Fact]
    public void GetRequiredCapabilityCodes_WithValidFilter_ShouldReturnCapabilityCodes()
    {
        // Arrange
        var requiredCapabilities = new List<string> { "capability1", "capability2" };
        var filter = AudienceFilter.FromDefinitionCodes(new List<string>(), requiredCapabilities);

        // Act
        var capabilityCodes = filter.GetRequiredCapabilityCodes();

        // Assert
        capabilityCodes.Should().BeEquivalentTo(requiredCapabilities);
    }

    [Fact]
    public void GetRequiredCapabilityCodes_WithEmptyFilter_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = AudienceFilter.FromDefinitionCodes(new List<string>(), new List<string>());

        // Act
        var capabilityCodes = filter.GetRequiredCapabilityCodes();

        // Assert
        capabilityCodes.Should().BeEmpty();
    }

    [Fact]
    public void IsEmpty_WithEmptyCriteria_ShouldReturnTrue()
    {
        // Arrange
        var filter = AudienceFilter.FromDefinitionCodes(new List<string>(), new List<string>());

        // Act & Assert
        filter.IsEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_WithNonEmptyCriteria_ShouldReturnFalse()
    {
        // Arrange
        var filter = AudienceFilter.FromDefinitionCodes(new List<string> { "feature1" }, new List<string>());

        // Act & Assert
        filter.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_WithEmptyArrays_ShouldReturnTrue()
    {
        // Arrange
        var emptyCriteria = new Dictionary<string, object>
        {
            ["requiredFeatures"] = new List<string>(),
            ["requiredCapabilities"] = new List<string>()
        };
        var filterExpression = JsonSerializer.Serialize(emptyCriteria);
        var filter = new AudienceFilter(filterExpression);

        // Act & Assert
        filter.IsEmpty().Should().BeTrue();
    }

    [Fact]
    public void Equality_WithSameExpression_ShouldBeEqual()
    {
        // Arrange
        var expression = """{"requiredFeatures": ["feature1"]}""";
        var filter1 = new AudienceFilter(expression);
        var filter2 = new AudienceFilter(expression);

        // Act & Assert
        filter1.Should().Be(filter2);
        filter1.GetHashCode().Should().Be(filter2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentExpressions_ShouldNotBeEqual()
    {
        // Arrange
        var filter1 = new AudienceFilter("""{"requiredFeatures": ["feature1"]}""");
        var filter2 = new AudienceFilter("""{"requiredFeatures": ["feature2"]}""");

        // Act & Assert
        filter1.Should().NotBe(filter2);
    }

    [Fact]
    public void Equality_WithDifferentVersions_ShouldNotBeEqual()
    {
        // Arrange
        var expression = """{"requiredFeatures": ["feature1"]}""";
        var filter1 = new AudienceFilter(expression, 1);
        var filter2 = new AudienceFilter(expression, 2);

        // Act & Assert
        filter1.Should().NotBe(filter2);
    }

    [Fact]
    public void BusinessRules_ComplexFilter_ShouldWorkCorrectly()
    {
        // Arrange
        var requiredFeatures = new List<string> { "has_license", "is_premium" };
        var requiredCapabilities = new List<string> { "can_export", "can_analyze" };
        var excludedFeatures = new List<string> { "is_trial" };
        var excludedCapabilities = new List<string> { "is_restricted" };

        // Act
        var filter = AudienceFilter.FromDefinitionCodes(
            requiredFeatures, 
            requiredCapabilities, 
            excludedFeatures, 
            excludedCapabilities);

        // Assert
        filter.GetRequiredFeatureCodes().Should().BeEquivalentTo(requiredFeatures);
        filter.GetRequiredCapabilityCodes().Should().BeEquivalentTo(requiredCapabilities);
        filter.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void BusinessRules_MemberGroupsFilter_ShouldWorkCorrectly()
    {
        // Arrange
        var memberGroups = new List<string> { "engineers", "managers", "seniors" };

        // Act
        var filter = AudienceFilter.ForMemberGroups(memberGroups);

        // Assert
        var criteria = filter.GetCriteria();
        criteria.Should().ContainKey("memberGroups");
        filter.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void BusinessRules_JsonSerialization_ShouldBeConsistent()
    {
        // Arrange
        var originalCriteria = new Dictionary<string, object>
        {
            ["requiredFeatures"] = new List<string> { "feature1", "feature2" },
            ["requiredCapabilities"] = new List<string> { "capability1" }
        };

        // Act
        var filter = new AudienceFilter(JsonSerializer.Serialize(originalCriteria));
        var parsedCriteria = filter.GetCriteria();

        // Assert
        parsedCriteria.Should().ContainKey("requiredFeatures");
        parsedCriteria.Should().ContainKey("requiredCapabilities");
    }
}
