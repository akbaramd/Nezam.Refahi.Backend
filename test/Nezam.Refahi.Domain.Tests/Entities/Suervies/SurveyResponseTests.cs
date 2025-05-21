using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Suervies;

public class SurveyResponseTests
{
    private readonly Survey _survey;
    private readonly User _responder;
    
    public SurveyResponseTests()
    {
        // Create a standard survey and responder for use in tests
        var creator = new User("علی", "محمدی", "2741153671", "09123456789");
        _survey = new Survey("نظرسنجی آزمایشی", "توضیحات نظرسنجی", SurveyMode.QnA, creator, DateTimeOffset.UtcNow);
        _responder = new User("رضا", "احمدی", "0074125678", "09187654321");
    }
    
    [Fact]
    public void SurveyResponse_Creation_Sets_Required_Properties()
    {
        // Act
        var response = new SurveyResponse(_survey, _responder);

        // Assert
        Assert.Equal(_survey.Id, response.SurveyId);
        Assert.Same(_survey, response.Survey);
        Assert.Equal(_responder.Id, response.ResponderId);
        Assert.Same(_responder, response.Responder);
        Assert.NotEqual(default, response.StartedAtUtc);
        Assert.Null(response.SubmittedAtUtc);
        Assert.False(response.TimedOut);
        Assert.Empty(response.Answers);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.NotEqual(default, response.CreatedAt);
        Assert.Null(response.ModifiedAt);
    }
    
    [Fact]
    public void SurveyResponse_Creation_With_Null_Responder_Creates_Anonymous_Response()
    {
        // Act
        var response = new SurveyResponse(_survey);

        // Assert
        Assert.Equal(_survey.Id, response.SurveyId);
        Assert.Same(_survey, response.Survey);
        Assert.Null(response.ResponderId);
        Assert.Null(response.Responder);
        Assert.NotEqual(default, response.StartedAtUtc);
    }
    
    [Fact]
    public void SurveyResponse_Creation_With_Null_Survey_Throws_ArgumentNullException()
    {
        // Arrange
        Survey? nullSurvey = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SurveyResponse(nullSurvey!));
    }
    
    [Fact]
    public void Submit_Sets_SubmittedAtUtc_TimedOut_And_ModifiedAt()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        Assert.Null(response.SubmittedAtUtc);
        Assert.False(response.TimedOut);

        // Act
        response.Submit(true); // Within time limit

        // Assert
        Assert.NotNull(response.SubmittedAtUtc);
        Assert.False(response.TimedOut);
        Assert.NotNull(response.ModifiedAt);
    }
    
    [Fact]
    public void Submit_With_TimedOut_Flag_Sets_TimedOut_To_True()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);

        // Act
        response.Submit(false); // Not within time limit

        // Assert
        Assert.NotNull(response.SubmittedAtUtc);
        Assert.True(response.TimedOut);
        Assert.NotNull(response.ModifiedAt);
    }
    
    [Fact]
    public void Submit_When_Already_Submitted_Does_Not_Change_SubmittedAtUtc()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        response.Submit(true);
        var originalSubmitTime = response.SubmittedAtUtc;

        // Reset ModifiedAt for clean testing
        typeof(SurveyResponse).GetProperty("ModifiedAt")?.SetValue(response, null);
        Assert.Null(response.ModifiedAt);
        
        // Wait a bit to ensure the timestamps would be different
        System.Threading.Thread.Sleep(10);
        
        // Act
        response.Submit(false); // Try to submit again with different time limit status

        // Assert
        Assert.Equal(originalSubmitTime, response.SubmittedAtUtc); // Should not change
        Assert.False(response.TimedOut); // Should not change from original value (true)
        Assert.Null(response.ModifiedAt); // No change was made
    }
    
    [Fact]
    public void MarkAsTimedOut_Sets_TimedOut_SubmittedAtUtc_And_ModifiedAt()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        Assert.Null(response.SubmittedAtUtc);
        Assert.False(response.TimedOut);

        // Act
        response.MarkAsTimedOut();

        // Assert
        Assert.NotNull(response.SubmittedAtUtc);
        Assert.True(response.TimedOut);
        Assert.NotNull(response.ModifiedAt);
    }
    
    [Fact]
    public void MarkAsTimedOut_When_Already_Submitted_Does_Not_Change_Status()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        response.Submit(true); // Submit with timedOut=false
        var originalSubmitTime = response.SubmittedAtUtc;
        Assert.False(response.TimedOut);

        // Reset ModifiedAt for clean testing
        typeof(SurveyResponse).GetProperty("ModifiedAt")?.SetValue(response, null);
        Assert.Null(response.ModifiedAt);
        
        // Act
        response.MarkAsTimedOut();

        // Assert
        Assert.Equal(originalSubmitTime, response.SubmittedAtUtc); // Should not change
        Assert.False(response.TimedOut); // Should not change
        Assert.Null(response.ModifiedAt); // No change was made
    }
    
    [Fact]
    public void AddAnswer_Adds_Answer_To_Response()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        var question = new SurveyQuestion(_survey, "سوال آزمایشی", QuestionType.OpenText, 0);
        var answer = new SurveyAnswer(response, question, null, "پاسخ آزمایشی");
        
        // Act
        response.AddAnswer(answer);
        
        // Assert
        Assert.Single(response.Answers);
        Assert.Contains(answer, response.Answers);
        Assert.NotNull(response.ModifiedAt);
    }
    
    [Fact]
    public void AddAnswer_When_Already_Submitted_Throws_InvalidOperationException()
    {
        // Arrange
        var response = new SurveyResponse(_survey, _responder);
        response.Submit(true); // Submit the response
        
        var question = new SurveyQuestion(_survey, "سوال آزمایشی", QuestionType.OpenText, 0);
        var answer = new SurveyAnswer(response, question, null, "پاسخ آزمایشی");
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => response.AddAnswer(answer));
    }
}
