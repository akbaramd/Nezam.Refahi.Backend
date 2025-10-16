using FluentAssertions;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Surveying.Tests.Domain.ValueObjects;

/// <summary>
/// Comprehensive tests for RepeatPolicy value object
/// Tests domain principles: immutability, validation, business rules, equality
/// </summary>
public class RepeatPolicyTests
{
    [Fact]
    public void None_ShouldCreateNonePolicy()
    {
        // Act
        var policy = RepeatPolicy.None();

        // Assert
        policy.Should().NotBeNull();
        policy.Kind.Should().Be(RepeatPolicyKind.None);
        policy.MaxRepeats.Should().BeNull();
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(2).Should().BeFalse();
        policy.GetMaxRepeatIndex().Should().Be(1);
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(1).Should().BeFalse();
        policy.ToString().Should().Be("None (Single)");
    }

    [Fact]
    public void Bounded_WithValidMaxRepeats_ShouldCreateBoundedPolicy()
    {
        // Arrange
        var maxRepeats = 3;

        // Act
        var policy = RepeatPolicy.Bounded(maxRepeats);

        // Assert
        policy.Should().NotBeNull();
        policy.Kind.Should().Be(RepeatPolicyKind.Bounded);
        policy.MaxRepeats.Should().Be(maxRepeats);
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(2).Should().BeTrue();
        policy.IsValidRepeatIndex(3).Should().BeTrue();
        policy.IsValidRepeatIndex(4).Should().BeFalse();
        policy.GetMaxRepeatIndex().Should().Be(maxRepeats);
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(1).Should().BeTrue();
        policy.CanAddMoreRepeats(2).Should().BeTrue();
        policy.CanAddMoreRepeats(3).Should().BeFalse();
        policy.ToString().Should().Be("Bounded (Max: 3)");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Bounded_WithInvalidMaxRepeats_ShouldThrowArgumentException(int maxRepeats)
    {
        // Act & Assert
        var action = () => RepeatPolicy.Bounded(maxRepeats);
        action.Should().Throw<ArgumentException>()
            .WithMessage("MaxRepeats must be at least 1*");
    }

    [Fact]
    public void Unbounded_ShouldCreateUnboundedPolicy()
    {
        // Act
        var policy = RepeatPolicy.Unbounded();

        // Assert
        policy.Should().NotBeNull();
        policy.Kind.Should().Be(RepeatPolicyKind.Unbounded);
        policy.MaxRepeats.Should().BeNull();
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(10).Should().BeTrue();
        policy.IsValidRepeatIndex(100).Should().BeTrue();
        policy.GetMaxRepeatIndex().Should().BeNull();
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(100).Should().BeTrue();
        policy.ToString().Should().Be("Unbounded (Unlimited)");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void IsValidRepeatIndex_WithInvalidIndex_ShouldReturnFalse(int repeatIndex)
    {
        // Arrange
        var nonePolicy = RepeatPolicy.None();
        var boundedPolicy = RepeatPolicy.Bounded(3);
        var unboundedPolicy = RepeatPolicy.Unbounded();

        // Act & Assert
        nonePolicy.IsValidRepeatIndex(repeatIndex).Should().BeFalse();
        boundedPolicy.IsValidRepeatIndex(repeatIndex).Should().BeFalse();
        unboundedPolicy.IsValidRepeatIndex(repeatIndex).Should().BeFalse();
    }

    [Fact]
    public void IsValidRepeatIndex_WithNonePolicy_ShouldOnlyAllowIndex1()
    {
        // Arrange
        var policy = RepeatPolicy.None();

        // Act & Assert
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(2).Should().BeFalse();
        policy.IsValidRepeatIndex(3).Should().BeFalse();
    }

    [Fact]
    public void IsValidRepeatIndex_WithBoundedPolicy_ShouldAllowUpToMaxRepeats()
    {
        // Arrange
        var policy = RepeatPolicy.Bounded(3);

        // Act & Assert
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(2).Should().BeTrue();
        policy.IsValidRepeatIndex(3).Should().BeTrue();
        policy.IsValidRepeatIndex(4).Should().BeFalse();
        policy.IsValidRepeatIndex(5).Should().BeFalse();
    }

    [Fact]
    public void IsValidRepeatIndex_WithUnboundedPolicy_ShouldAllowAnyPositiveIndex()
    {
        // Arrange
        var policy = RepeatPolicy.Unbounded();

        // Act & Assert
        policy.IsValidRepeatIndex(1).Should().BeTrue();
        policy.IsValidRepeatIndex(10).Should().BeTrue();
        policy.IsValidRepeatIndex(100).Should().BeTrue();
        policy.IsValidRepeatIndex(1000).Should().BeTrue();
    }

    [Fact]
    public void GetMaxRepeatIndex_WithNonePolicy_ShouldReturn1()
    {
        // Arrange
        var policy = RepeatPolicy.None();

        // Act & Assert
        policy.GetMaxRepeatIndex().Should().Be(1);
    }

    [Fact]
    public void GetMaxRepeatIndex_WithBoundedPolicy_ShouldReturnMaxRepeats()
    {
        // Arrange
        var policy = RepeatPolicy.Bounded(5);

        // Act & Assert
        policy.GetMaxRepeatIndex().Should().Be(5);
    }

    [Fact]
    public void GetMaxRepeatIndex_WithUnboundedPolicy_ShouldReturnNull()
    {
        // Arrange
        var policy = RepeatPolicy.Unbounded();

        // Act & Assert
        policy.GetMaxRepeatIndex().Should().BeNull();
    }

    [Fact]
    public void CanAddMoreRepeats_WithNonePolicy_ShouldOnlyAllowFirstRepeat()
    {
        // Arrange
        var policy = RepeatPolicy.None();

        // Act & Assert
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(1).Should().BeFalse();
        policy.CanAddMoreRepeats(2).Should().BeFalse();
    }

    [Fact]
    public void CanAddMoreRepeats_WithBoundedPolicy_ShouldAllowUpToMaxRepeats()
    {
        // Arrange
        var policy = RepeatPolicy.Bounded(3);

        // Act & Assert
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(1).Should().BeTrue();
        policy.CanAddMoreRepeats(2).Should().BeTrue();
        policy.CanAddMoreRepeats(3).Should().BeFalse();
        policy.CanAddMoreRepeats(4).Should().BeFalse();
    }

    [Fact]
    public void CanAddMoreRepeats_WithUnboundedPolicy_ShouldAlwaysAllow()
    {
        // Arrange
        var policy = RepeatPolicy.Unbounded();

        // Act & Assert
        policy.CanAddMoreRepeats(0).Should().BeTrue();
        policy.CanAddMoreRepeats(1).Should().BeTrue();
        policy.CanAddMoreRepeats(100).Should().BeTrue();
        policy.CanAddMoreRepeats(1000).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnCorrectRepresentation()
    {
        // Arrange
        var nonePolicy = RepeatPolicy.None();
        var boundedPolicy = RepeatPolicy.Bounded(5);
        var unboundedPolicy = RepeatPolicy.Unbounded();

        // Act & Assert
        nonePolicy.ToString().Should().Be("None (Single)");
        boundedPolicy.ToString().Should().Be("Bounded (Max: 5)");
        unboundedPolicy.ToString().Should().Be("Unbounded (Unlimited)");
    }

    [Fact]
    public void BusinessRules_EdgeCases_ShouldWorkCorrectly()
    {
        // Arrange
        var singleRepeatPolicy = RepeatPolicy.Bounded(1);
        var largeBoundedPolicy = RepeatPolicy.Bounded(1000);

        // Act & Assert
        // Single repeat
        singleRepeatPolicy.IsValidRepeatIndex(1).Should().BeTrue();
        singleRepeatPolicy.IsValidRepeatIndex(2).Should().BeFalse();
        singleRepeatPolicy.CanAddMoreRepeats(0).Should().BeTrue();
        singleRepeatPolicy.CanAddMoreRepeats(1).Should().BeFalse();

        // Large bounded
        largeBoundedPolicy.IsValidRepeatIndex(1000).Should().BeTrue();
        largeBoundedPolicy.IsValidRepeatIndex(1001).Should().BeFalse();
        largeBoundedPolicy.CanAddMoreRepeats(999).Should().BeTrue();
        largeBoundedPolicy.CanAddMoreRepeats(1000).Should().BeFalse();
    }

    [Fact]
    public void BusinessRules_ConsistencyBetweenMethods_ShouldBeConsistent()
    {
        // Arrange
        var policy = RepeatPolicy.Bounded(3);

        // Act & Assert
        // If we can add more repeats, the next index should be valid
        for (int i = 0; i < 3; i++)
        {
            if (policy.CanAddMoreRepeats(i))
            {
                policy.IsValidRepeatIndex(i + 1).Should().BeTrue();
            }
        }

        // If we can't add more repeats, the next index should be invalid
        policy.CanAddMoreRepeats(3).Should().BeFalse();
        policy.IsValidRepeatIndex(4).Should().BeFalse();
    }
}
