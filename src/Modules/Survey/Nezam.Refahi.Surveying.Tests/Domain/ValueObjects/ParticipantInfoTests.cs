using FluentAssertions;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Xunit;

namespace Nezam.Refahi.Surveying.Tests.Domain.ValueObjects;

/// <summary>
/// Comprehensive tests for ParticipantInfo value object
/// Tests domain principles: immutability, validation, equality, business rules
/// </summary>
public class ParticipantInfoTests
{
    [Fact]
    public void ForMember_WithValidMemberId_ShouldCreateParticipantInfo()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        // Act
        var participantInfo = ParticipantInfo.ForMember(memberId);

        // Assert
        participantInfo.Should().NotBeNull();
        participantInfo.IsAnonymous.Should().BeFalse();
        participantInfo.MemberId.Should().Be(memberId);
        participantInfo.ParticipantHash.Should().BeNull();
        participantInfo.GetParticipantIdentifier().Should().Be(memberId.ToString());
        participantInfo.GetDisplayName().Should().Be($"Member {memberId}");
        participantInfo.GetShortIdentifier().Should().Be(memberId.ToString()[..8]);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void ForMember_WithEmptyGuid_ShouldThrowArgumentException(string memberIdString)
    {
        // Arrange
        var memberId = Guid.Parse(memberIdString);

        // Act & Assert
        var action = () => ParticipantInfo.ForMember(memberId);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Member ID cannot be empty*");
    }

    [Fact]
    public void ForAnonymous_WithValidHash_ShouldCreateParticipantInfo()
    {
        // Arrange
        var participantHash = "abc123def456";

        // Act
        var participantInfo = ParticipantInfo.ForAnonymous(participantHash);

        // Assert
        participantInfo.Should().NotBeNull();
        participantInfo.IsAnonymous.Should().BeTrue();
        participantInfo.MemberId.Should().BeNull();
        participantInfo.ParticipantHash.Should().Be(participantHash);
        participantInfo.GetParticipantIdentifier().Should().Be(participantHash);
        participantInfo.GetDisplayName().Should().Be($"Anonymous ({participantHash[..8]})");
        participantInfo.GetShortIdentifier().Should().Be(participantHash[..8]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ForAnonymous_WithInvalidHash_ShouldThrowArgumentException(string? participantHash)
    {
        // Act & Assert
        var action = () => ParticipantInfo.ForAnonymous(participantHash!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Participant hash cannot be empty*");
    }

    [Fact]
    public void ForAnonymous_WithWhitespaceHash_ShouldTrimHash()
    {
        // Arrange
        var participantHash = "  abc123def456  ";

        // Act
        var participantInfo = ParticipantInfo.ForAnonymous(participantHash);

        // Assert
        participantInfo.ParticipantHash.Should().Be("abc123def456");
    }

    [Fact]
    public void Equality_WithSameMemberId_ShouldBeEqual()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var participant1 = ParticipantInfo.ForMember(memberId);
        var participant2 = ParticipantInfo.ForMember(memberId);

        // Act & Assert
        participant1.Should().Be(participant2);
        participant1.GetHashCode().Should().Be(participant2.GetHashCode());
    }

    [Fact]
    public void Equality_WithSameParticipantHash_ShouldBeEqual()
    {
        // Arrange
        var hash = "abc123def456";
        var participant1 = ParticipantInfo.ForAnonymous(hash);
        var participant2 = ParticipantInfo.ForAnonymous(hash);

        // Act & Assert
        participant1.Should().Be(participant2);
        participant1.GetHashCode().Should().Be(participant2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentMemberIds_ShouldNotBeEqual()
    {
        // Arrange
        var participant1 = ParticipantInfo.ForMember(Guid.NewGuid());
        var participant2 = ParticipantInfo.ForMember(Guid.NewGuid());

        // Act & Assert
        participant1.Should().NotBe(participant2);
    }

    [Fact]
    public void Equality_WithDifferentParticipantHashes_ShouldNotBeEqual()
    {
        // Arrange
        var participant1 = ParticipantInfo.ForAnonymous("hash1");
        var participant2 = ParticipantInfo.ForAnonymous("hash2");

        // Act & Assert
        participant1.Should().NotBe(participant2);
    }

    [Fact]
    public void Equality_MemberVsAnonymous_ShouldNotBeEqual()
    {
        // Arrange
        var memberParticipant = ParticipantInfo.ForMember(Guid.NewGuid());
        var anonymousParticipant = ParticipantInfo.ForAnonymous("hash");

        // Act & Assert
        memberParticipant.Should().NotBe(anonymousParticipant);
    }

    [Fact]
    public void GetShortIdentifier_WithLongHash_ShouldReturnFirst8Characters()
    {
        // Arrange
        var longHash = "abcdefghijklmnopqrstuvwxyz";
        var participant = ParticipantInfo.ForAnonymous(longHash);

        // Act
        var shortId = participant.GetShortIdentifier();

        // Assert
        shortId.Should().Be("abcdefgh");
    }

    [Fact]
    public void GetShortIdentifier_WithShortHash_ShouldReturnFullHash()
    {
        // Arrange
        var shortHash = "abc12345"; // 8 characters to match the substring length
        var participant = ParticipantInfo.ForAnonymous(shortHash);

        // Act
        var shortId = participant.GetShortIdentifier();

        // Assert
        shortId.Should().Be("abc12345");
    }

    [Fact]
    public void GetShortIdentifier_WithMemberId_ShouldReturnFirst8CharactersOfGuid()
    {
        // Arrange
        var memberId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var participant = ParticipantInfo.ForMember(memberId);

        // Act
        var shortId = participant.GetShortIdentifier();

        // Assert
        shortId.Should().Be("12345678");
    }
}
