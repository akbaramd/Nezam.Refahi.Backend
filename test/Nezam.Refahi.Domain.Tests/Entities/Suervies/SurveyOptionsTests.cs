using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Suervies;

public class SurveyOptionsTests
{
    private readonly SurveyQuestion _question;
    
    public SurveyOptionsTests()
    {
        // Create a standard question for use in tests
        var creator = new User("علی", "محمدی", "2741153671", "09123456789");
        var survey = new Survey("نظرسنجی آزمایشی", "توضیحات نظرسنجی", SurveyMode.QnA, creator, DateTimeOffset.UtcNow);
        _question = new SurveyQuestion(survey, "سوال آزمایشی", QuestionType.SingleChoice, 0);
    }
    
    [Fact]
    public void SurveyOptions_Creation_Sets_Required_Properties()
    {
        // Arrange
        string text = "گزینه اول";
        int displayOrder = 0;

        // Act
        var option = new SurveyOptions(_question, text, displayOrder);

        // Assert
        Assert.Equal(text, option.Text);
        Assert.Equal(displayOrder, option.DisplayOrder);
        Assert.Equal(_question.Id, option.QuestionId);
        Assert.Same(_question, option.Question);
        Assert.NotEqual(Guid.Empty, option.Id);
        Assert.NotEqual(default, option.CreatedAt);
        Assert.Null(option.ModifiedAt);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SurveyOptions_Creation_With_Empty_Text_Throws_ArgumentException(string text)
    {
        // Arrange
        int displayOrder = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SurveyOptions(_question, text, displayOrder));
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void SurveyOptions_Creation_With_Negative_DisplayOrder_Throws_ArgumentException(int displayOrder)
    {
        // Arrange
        string text = "گزینه اول";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SurveyOptions(_question, text, displayOrder));
    }
    
    [Fact]
    public void SurveyOptions_Creation_With_Null_Question_Throws_ArgumentNullException()
    {
        // Arrange
        string text = "گزینه اول";
        int displayOrder = 0;
        SurveyQuestion? nullQuestion = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SurveyOptions(nullQuestion!, text, displayOrder));
    }
    
    [Fact]
    public void UpdateText_Updates_Text_And_ModifiedAt()
    {
        // Arrange
        var option = new SurveyOptions(_question, "گزینه اولیه", 0);
        string newText = "گزینه به‌روز شده";

        // Act
        option.UpdateText(newText);

        // Assert
        Assert.Equal(newText, option.Text);
        Assert.NotNull(option.ModifiedAt);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateText_With_Empty_Text_Throws_ArgumentException(string newText)
    {
        // Arrange
        var option = new SurveyOptions(_question, "گزینه اولیه", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => option.UpdateText(newText));
    }
    
    [Fact]
    public void UpdateDisplayOrder_Updates_DisplayOrder_And_ModifiedAt()
    {
        // Arrange
        var option = new SurveyOptions(_question, "گزینه اولیه", 0);
        int newDisplayOrder = 3;

        // Act
        option.UpdateDisplayOrder(newDisplayOrder);

        // Assert
        Assert.Equal(newDisplayOrder, option.DisplayOrder);
        Assert.NotNull(option.ModifiedAt);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateDisplayOrder_With_Negative_Order_Throws_ArgumentException(int newDisplayOrder)
    {
        // Arrange
        var option = new SurveyOptions(_question, "گزینه اولیه", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => option.UpdateDisplayOrder(newDisplayOrder));
    }
}
