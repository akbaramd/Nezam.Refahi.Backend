using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Suervies;

public class SurveyTests
{
    private readonly User _creator;
    
    public SurveyTests()
    {
        // Create a standard creator for use in tests
        _creator = new User("علی", "محمدی", "2741153671", "09123456789");
    }
    
    [Fact]
    public void Survey_Creation_Sets_Required_Properties()
    {
        // Arrange
        string title = "نظرسنجی رستوران";
        string description = "نظرسنجی کیفیت غذای رستوران";
        SurveyMode mode = SurveyMode.QnA;
        var opensAt = DateTimeOffset.UtcNow.AddDays(1);
        var closesAt = DateTimeOffset.UtcNow.AddDays(7);
        int? timeLimit = 300;

        // Act
        var survey = new Survey(title, description, mode, _creator, opensAt, closesAt, timeLimit);

        // Assert
        Assert.Equal(title, survey.Title);
        Assert.Equal(description, survey.Description);
        Assert.Equal(mode, survey.Mode);
        Assert.Equal(SurveyStatus.Draft, survey.Status);
        Assert.Equal(opensAt, survey.OpensAtUtc);
        Assert.Equal(closesAt, survey.ClosesAtUtc);
        Assert.Equal(timeLimit, survey.TimeLimitSeconds);
        Assert.Equal(_creator.Id, survey.CreatorId);
        Assert.Same(_creator, survey.Creator);
        Assert.NotEqual(Guid.Empty, survey.Id);
        Assert.NotEqual(default, survey.CreatedAt);
        Assert.Null(survey.ModifiedAt);
        Assert.Empty(survey.Questions);
        Assert.Empty(survey.Responses);
    }

    [Fact]
    public void Survey_Creation_With_Only_Required_Fields_Succeeds()
    {
        // Arrange
        string title = "نظرسنجی رستوران";
        SurveyMode mode = SurveyMode.Poll;
        var opensAt = DateTimeOffset.UtcNow;

        // Act
        var survey = new Survey(title, null, mode, _creator, opensAt);

        // Assert
        Assert.Equal(title, survey.Title);
        Assert.Null(survey.Description);
        Assert.Equal(mode, survey.Mode);
        Assert.Equal(SurveyStatus.Draft, survey.Status);
        Assert.Equal(opensAt, survey.OpensAtUtc);
        Assert.Null(survey.ClosesAtUtc);
        Assert.Null(survey.TimeLimitSeconds);
        Assert.Equal(_creator.Id, survey.CreatorId);
        Assert.Same(_creator, survey.Creator);
    }

    [Theory]
    [InlineData(null, "نظرسنجی رضایت کارکنان")]
    [InlineData("", "نظرسنجی رضایت کارکنان")]
    [InlineData("  ", "نظرسنجی رضایت کارکنان")]
    public void Survey_Creation_With_Missing_Title_Throws_ArgumentException(string title, string description)
    {
        // Arrange
        SurveyMode mode = SurveyMode.QnA;
        var opensAt = DateTimeOffset.UtcNow;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Survey(title, description, mode, _creator, opensAt));
    }
    
    [Fact]
    public void Survey_Creation_With_Null_Creator_Throws_ArgumentNullException()
    {
        // Arrange
        string title = "نظرسنجی رستوران";
        string description = "نظرسنجی کیفیت غذای رستوران";
        SurveyMode mode = SurveyMode.QnA;
        var opensAt = DateTimeOffset.UtcNow;
        User? nullCreator = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Survey(title, description, mode, nullCreator!, opensAt));
    }

    [Fact]
    public void UpdateTitle_Updates_Title_And_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی اولیه", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        string newTitle = "نظرسنجی به‌روز شده";

        // Act
        survey.UpdateTitle(newTitle);

        // Assert
        Assert.Equal(newTitle, survey.Title);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateTitle_With_Invalid_Title_Throws_ArgumentException(string newTitle)
    {
        // Arrange
        var survey = new Survey("نظرسنجی اولیه", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => survey.UpdateTitle(newTitle));
    }

    [Fact]
    public void UpdateDescription_Updates_Description_And_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        string newDescription = "توضیحات جدید";

        // Act
        survey.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, survey.Description);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void UpdateDescription_Can_Set_Null_Description()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", "توضیحات", SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);

        // Act
        survey.UpdateDescription(null);

        // Assert
        Assert.Null(survey.Description);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void UpdateSchedule_Updates_OpeningAndClosing_Times_And_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        var newOpensAt = DateTimeOffset.UtcNow.AddDays(3);
        var newClosesAt = DateTimeOffset.UtcNow.AddDays(10);

        // Act
        survey.UpdateSchedule(newOpensAt, newClosesAt);

        // Assert
        Assert.Equal(newOpensAt, survey.OpensAtUtc);
        Assert.Equal(newClosesAt, survey.ClosesAtUtc);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void UpdateTimeLimit_Updates_TimeLimit_And_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        int? newTimeLimit = 600;

        // Act
        survey.UpdateTimeLimit(newTimeLimit);

        // Assert
        Assert.Equal(newTimeLimit, survey.TimeLimitSeconds);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void UpdateTimeLimit_Can_Set_Null_TimeLimit()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow, timeLimitSeconds: 300);

        // Act
        survey.UpdateTimeLimit(null);

        // Assert
        Assert.Null(survey.TimeLimitSeconds);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void UpdateStatus_Updates_Status_And_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        var newStatus = SurveyStatus.Scheduled;

        // Act
        survey.UpdateStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, survey.Status);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void Publish_Changes_Status_To_Published_And_Updates_ModifiedAt()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        Assert.Equal(SurveyStatus.Draft, survey.Status);

        // Act
        survey.Publish();

        // Assert
        Assert.Equal(SurveyStatus.Published, survey.Status);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void Publish_From_Scheduled_Changes_Status_To_Published()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        survey.UpdateStatus(SurveyStatus.Scheduled);
        Assert.Equal(SurveyStatus.Scheduled, survey.Status);

        // Act
        survey.Publish();

        // Assert
        Assert.Equal(SurveyStatus.Published, survey.Status);
    }

    [Fact]
    public void Close_From_Published_Changes_Status_To_Closed()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        survey.Publish();
        Assert.Equal(SurveyStatus.Published, survey.Status);

        // Act
        survey.Close();

        // Assert
        Assert.Equal(SurveyStatus.Closed, survey.Status);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void Close_From_NonPublished_DoesNotChange_Status()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        Assert.Equal(SurveyStatus.Draft, survey.Status);

        // Act
        survey.Close();

        // Assert
        Assert.Equal(SurveyStatus.Draft, survey.Status); // Should remain Draft
    }

    [Fact]
    public void Archive_From_Closed_Changes_Status_To_Archived()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        survey.Publish();
        survey.Close();
        Assert.Equal(SurveyStatus.Closed, survey.Status);

        // Act
        survey.Archive();

        // Assert
        Assert.Equal(SurveyStatus.Archived, survey.Status);
        Assert.NotNull(survey.ModifiedAt);
    }

    [Fact]
    public void Archive_From_NonClosed_DoesNotChange_Status()
    {
        // Arrange
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, DateTimeOffset.UtcNow);
        survey.Publish();
        Assert.Equal(SurveyStatus.Published, survey.Status);

        // Act
        survey.Archive();

        // Assert
        Assert.Equal(SurveyStatus.Published, survey.Status); // Should remain Published
    }

    [Fact]
    public void IsOpen_Returns_True_When_Published_And_Within_TimeWindow()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var opensAt = now.AddHours(-1);
        var closesAt = now.AddHours(1);
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, opensAt, closesAt);
        survey.Publish();

        // Act
        bool isOpen = survey.IsOpen(now);

        // Assert
        Assert.True(isOpen);
    }

    [Fact]
    public void IsOpen_Returns_True_When_Published_And_NoClosingDate()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var opensAt = now.AddHours(-1);
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, opensAt, null);
        survey.Publish();

        // Act
        bool isOpen = survey.IsOpen(now);

        // Assert
        Assert.True(isOpen);
    }

    [Fact]
    public void IsOpen_Returns_False_When_NotPublished()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var opensAt = now.AddHours(-1);
        var closesAt = now.AddHours(1);
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, opensAt, closesAt);
        // Not publishing the survey, remains in Draft status

        // Act
        bool isOpen = survey.IsOpen(now);

        // Assert
        Assert.False(isOpen);
    }

    [Fact]
    public void IsOpen_Returns_False_When_Published_But_BeforeOpeningTime()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var opensAt = now.AddHours(1); // Opens in the future
        var closesAt = now.AddHours(2);
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, opensAt, closesAt);
        survey.Publish();

        // Act
        bool isOpen = survey.IsOpen(now);

        // Assert
        Assert.False(isOpen);
    }

    [Fact]
    public void IsOpen_Returns_False_When_Published_But_AfterClosingTime()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var opensAt = now.AddHours(-2);
        var closesAt = now.AddHours(-1); // Closed in the past
        var survey = new Survey("نظرسنجی", null, SurveyMode.Poll, _creator, opensAt, closesAt);
        survey.Publish();

        // Act
        bool isOpen = survey.IsOpen(now);

        // Assert
        Assert.False(isOpen);
    }
}
