using FluentAssertions;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Surveying.Tests.Domain.ValueObjects;

/// <summary>
/// Comprehensive tests for ParticipationPolicy value object
/// Tests domain principles: immutability, validation, business rules, equality
/// </summary>
public class ParticipationPolicyTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePolicy()
    {
        // Arrange
        var maxAttempts = 3;
        var allowMultipleSubmissions = true;
        var coolDownSeconds = 300;
        var allowBackNavigation = false;

        // Act
        var policy = new ParticipationPolicy(maxAttempts, allowMultipleSubmissions, coolDownSeconds, allowBackNavigation);

        // Assert
        policy.Should().NotBeNull();
        policy.MaxAttemptsPerMember.Should().Be(maxAttempts);
        policy.AllowMultipleSubmissions.Should().Be(allowMultipleSubmissions);
        policy.CoolDownSeconds.Should().Be(coolDownSeconds);
        policy.AllowBackNavigation.Should().Be(allowBackNavigation);
    }

    [Fact]
    public void Constructor_WithMinimalParameters_ShouldCreatePolicy()
    {
        // Arrange
        var maxAttempts = 1;
        var allowMultipleSubmissions = false;

        // Act
        var policy = new ParticipationPolicy(maxAttempts, allowMultipleSubmissions);

        // Assert
        policy.Should().NotBeNull();
        policy.MaxAttemptsPerMember.Should().Be(maxAttempts);
        policy.AllowMultipleSubmissions.Should().Be(allowMultipleSubmissions);
        policy.CoolDownSeconds.Should().BeNull();
        policy.AllowBackNavigation.Should().BeTrue(); // Default value
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_WithInvalidMaxAttempts_ShouldThrowArgumentException(int maxAttempts)
    {
        // Act & Assert
        var action = () => new ParticipationPolicy(maxAttempts, true);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Max attempts per member must be greater than 0*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_WithNegativeCoolDown_ShouldThrowArgumentException(int coolDownSeconds)
    {
        // Act & Assert
        var action = () => new ParticipationPolicy(3, true, coolDownSeconds);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Cool down seconds cannot be negative*");
    }

    [Fact]
    public void Constructor_WithZeroCoolDown_ShouldBeValid()
    {
        // Act
        var policy = new ParticipationPolicy(3, true, 0);

        // Assert
        policy.CoolDownSeconds.Should().Be(0);
    }

    [Fact]
    public void IsAttemptAllowed_WithValidAttemptNumber_ShouldReturnTrue()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true);

        // Act & Assert
        policy.IsAttemptAllowed(0).Should().BeTrue();
        policy.IsAttemptAllowed(1).Should().BeTrue();
        policy.IsAttemptAllowed(2).Should().BeTrue();
    }

    [Fact]
    public void IsAttemptAllowed_WithMaxAttemptsReached_ShouldReturnFalse()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true);

        // Act & Assert
        policy.IsAttemptAllowed(3).Should().BeTrue();
        policy.IsAttemptAllowed(4).Should().BeFalse();
        policy.IsAttemptAllowed(10).Should().BeFalse();
    }

    [Fact]
    public void IsAttemptAllowed_WithNegativeAttemptNumber_ShouldReturnFalse()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true);

        // Act & Assert
        policy.IsAttemptAllowed(-1).Should().BeFalse();
        policy.IsAttemptAllowed(-10).Should().BeFalse();
    }

    [Fact]
    public void IsCoolDownPassed_WithNoCoolDown_ShouldReturnTrue()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, null);
        var lastAttemptTime = DateTime.UtcNow.AddMinutes(-1);

        // Act & Assert
        policy.IsCoolDownPassed(lastAttemptTime).Should().BeTrue();
        policy.IsCoolDownPassed(null).Should().BeTrue();
    }

    [Fact]
    public void IsCoolDownPassed_WithCoolDownNotPassed_ShouldReturnFalse()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, 300); // 5 minutes
        var lastAttemptTime = DateTime.UtcNow.AddMinutes(-2); // 2 minutes ago

        // Act & Assert
        policy.IsCoolDownPassed(lastAttemptTime).Should().BeFalse();
    }

    [Fact]
    public void IsCoolDownPassed_WithCoolDownPassed_ShouldReturnTrue()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, 300); // 5 minutes
        var lastAttemptTime = DateTime.UtcNow.AddMinutes(-6); // 6 minutes ago

        // Act & Assert
        policy.IsCoolDownPassed(lastAttemptTime).Should().BeTrue();
    }

    [Fact]
    public void IsCoolDownPassed_WithExactCoolDownTime_ShouldReturnTrue()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, 300); // 5 minutes
        var lastAttemptTime = DateTime.UtcNow.AddSeconds(-300); // Exactly 5 minutes ago

        // Act & Assert
        policy.IsCoolDownPassed(lastAttemptTime).Should().BeTrue();
    }

    [Fact]
    public void IsCoolDownPassed_WithNullLastAttemptTime_ShouldReturnTrue()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, 300);

        // Act & Assert
        policy.IsCoolDownPassed(null).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithSameParameters_ShouldBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, 300, false);
        var policy2 = new ParticipationPolicy(3, true, 300, false);

        // Act & Assert
        policy1.Should().Be(policy2);
        policy1.GetHashCode().Should().Be(policy2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentMaxAttempts_ShouldNotBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, 300, false);
        var policy2 = new ParticipationPolicy(5, true, 300, false);

        // Act & Assert
        policy1.Should().NotBe(policy2);
    }

    [Fact]
    public void Equality_WithDifferentAllowMultipleSubmissions_ShouldNotBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, 300, false);
        var policy2 = new ParticipationPolicy(3, false, 300, false);

        // Act & Assert
        policy1.Should().NotBe(policy2);
    }

    [Fact]
    public void Equality_WithDifferentCoolDownSeconds_ShouldNotBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, 300, false);
        var policy2 = new ParticipationPolicy(3, true, 600, false);

        // Act & Assert
        policy1.Should().NotBe(policy2);
    }

    [Fact]
    public void Equality_WithNullVsZeroCoolDown_ShouldBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, null, false);
        var policy2 = new ParticipationPolicy(3, true, 0, false);

        // Act & Assert
        policy1.Should().Be(policy2);
    }

    [Fact]
    public void Equality_WithBothNullCoolDown_ShouldBeEqual()
    {
        // Arrange
        var policy1 = new ParticipationPolicy(3, true, null, false);
        var policy2 = new ParticipationPolicy(3, true, null, false);

        // Act & Assert
        policy1.Should().Be(policy2);
    }

    [Fact]
    public void BusinessRules_UnlimitedAttempts_ShouldWorkCorrectly()
    {
        // Arrange
        var policy = new ParticipationPolicy(int.MaxValue, true);

        // Act & Assert
        policy.IsAttemptAllowed(1000).Should().BeTrue();
        policy.IsAttemptAllowed(10000).Should().BeTrue();
    }

    [Fact]
    public void BusinessRules_SingleAttempt_ShouldWorkCorrectly()
    {
        // Arrange
        var policy = new ParticipationPolicy(1, false);

        // Act & Assert
        policy.IsAttemptAllowed(1).Should().BeTrue();
        policy.IsAttemptAllowed(2).Should().BeFalse();
    }

    [Fact]
    public void BusinessRules_NoCoolDown_ShouldAlwaysAllow()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, null);
        var recentTime = DateTime.UtcNow.AddSeconds(-1);

        // Act & Assert
        policy.IsCoolDownPassed(recentTime).Should().BeTrue();
        policy.IsCoolDownPassed(null).Should().BeTrue();
    }

    [Fact]
    public void BusinessRules_ImmediateCoolDown_ShouldWorkCorrectly()
    {
        // Arrange
        var policy = new ParticipationPolicy(3, true, 0);
        var recentTime = DateTime.UtcNow.AddSeconds(-1);

        // Act & Assert
        policy.IsCoolDownPassed(recentTime).Should().BeTrue();
        policy.IsCoolDownPassed(DateTime.UtcNow).Should().BeTrue();
    }
}
