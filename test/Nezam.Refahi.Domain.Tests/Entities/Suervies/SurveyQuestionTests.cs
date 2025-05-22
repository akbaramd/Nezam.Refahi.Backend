using System;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Suervies;

public class SurveyQuestionTests
{
    private readonly Survey _survey;
    
    public SurveyQuestionTests()
    {
        // Create a standard survey for use in tests
        var creator = new User("علی", "محمدی", "2741153671", "09123456789");
        _survey = new Survey("نظرسنجی آزمایشی", "توضیحات نظرسنجی", SurveyMode.QnA, creator, DateTimeOffset.UtcNow);
    }
    
    [Fact]
    public void SurveyQuestion_Creation_Sets_Required_Properties()
    {
        // Arrange
        string text = "آیا از کیفیت غذا راضی هستید؟";
        QuestionType type = QuestionType.SingleChoice;
        int order = 0;
        bool isRequired = true;

        // Act
        var question = new SurveyQuestion(_survey, text, type, order, isRequired);

        // Assert
        Assert.Equal(text, question.Text);
        Assert.Equal(type, question.Type);
        Assert.Equal(order, question.Order);
        Assert.Equal(isRequired, question.IsRequired);
        Assert.Equal(_survey.Id, question.SurveyId);
        Assert.Same(_survey, question.Survey);
        Assert.NotEqual(Guid.Empty, question.Id);
        Assert.NotEqual(default, question.CreatedAt);
        Assert.Null(question.ModifiedAt);
        Assert.Empty(question.Options);
        Assert.Empty(question.Answers);
    }
    
    [Fact]
    public void SurveyQuestion_Creation_With_Default_IsRequired_Succeeds()
    {
        // Arrange
        string text = "آیا از کیفیت غذا راضی هستید؟";
        QuestionType type = QuestionType.SingleChoice;
        int order = 0;

        // Act
        var question = new SurveyQuestion(_survey, text, type, order);

        // Assert
        Assert.Equal(text, question.Text);
        Assert.Equal(type, question.Type);
        Assert.Equal(order, question.Order);
        Assert.False(question.IsRequired); // Default value should be false
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SurveyQuestion_Creation_With_Empty_Text_Throws_ArgumentException(string text)
    {
        // Arrange
        QuestionType type = QuestionType.SingleChoice;
        int order = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SurveyQuestion(_survey, text, type, order));
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void SurveyQuestion_Creation_With_Negative_Order_Throws_ArgumentException(int order)
    {
        // Arrange
        string text = "آیا از کیفیت غذا راضی هستید؟";
        QuestionType type = QuestionType.SingleChoice;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SurveyQuestion(_survey, text, type, order));
    }
    
    [Fact]
    public void SurveyQuestion_Creation_With_Null_Survey_Throws_ArgumentNullException()
    {
        // Arrange
        string text = "آیا از کیفیت غذا راضی هستید؟";
        QuestionType type = QuestionType.SingleChoice;
        int order = 0;
        Survey nullSurvey = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SurveyQuestion(nullSurvey!, text, type, order));
    }
    
    [Fact]
    public void UpdateText_Updates_Text_And_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        string newText = "سوال به‌روز شده";

        // Act
        question.UpdateText(newText);

        // Assert
        Assert.Equal(newText, question.Text);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateText_With_Empty_Text_Throws_ArgumentException(string newText)
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => question.UpdateText(newText));
    }
    
    [Fact]
    public void ChangeType_Updates_Type_And_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        QuestionType newType = QuestionType.MultipleChoice;

        // Act
        question.ChangeType(newType);

        // Assert
        Assert.Equal(newType, question.Type);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Fact]
    public void ChangeType_From_Choice_To_OpenText_Clears_Options()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        // We need to add some options first to test they get cleared
        question.AddOption("گزینه 1", 0);
        question.AddOption("گزینه 2", 1);
        Assert.Equal(2, question.Options.Count);
        
        // Act
        question.ChangeType(QuestionType.OpenText);

        // Assert
        Assert.Equal(QuestionType.OpenText, question.Type);
        Assert.Empty(question.Options);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Fact]
    public void SetRequired_Updates_IsRequired_And_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0, false);
        Assert.False(question.IsRequired);

        // Act
        question.SetRequired(true);

        // Assert
        Assert.True(question.IsRequired);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Fact]
    public void UpdateOrder_Updates_Order_And_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        int newOrder = 3;

        // Act
        question.UpdateOrder(newOrder);

        // Assert
        Assert.Equal(newOrder, question.Order);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateOrder_With_Negative_Order_Throws_ArgumentException(int newOrder)
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => question.UpdateOrder(newOrder));
    }
    
    [Fact]
    public void AddOption_For_SingleChoice_Question_Adds_Option_And_Updates_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        string optionText = "گزینه جدید";
        int displayOrder = 0;

        // Act
        question.AddOption(optionText, displayOrder);

        // Assert
        Assert.Single(question.Options);
        var option = question.Options.First();
        Assert.Equal(optionText, option.Text);
        Assert.Equal(displayOrder, option.DisplayOrder);
        Assert.Equal(question.Id, option.QuestionId);
        Assert.Same(question, option.Question);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Fact]
    public void AddOption_For_MultipleChoice_Question_Adds_Option()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.MultipleChoice, 0);
        string optionText = "گزینه جدید";
        int displayOrder = 0;

        // Act
        question.AddOption(optionText, displayOrder);

        // Assert
        Assert.Single(question.Options);
    }
    
    [Fact]
    public void AddOption_For_Rating_Question_Adds_Option()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.Rating, 0);
        string optionText = "5 ستاره";
        int displayOrder = 0;

        // Act
        question.AddOption(optionText, displayOrder);

        // Assert
        Assert.Single(question.Options);
    }
    
    [Fact]
    public void AddOption_For_OpenText_Question_Throws_InvalidOperationException()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.OpenText, 0);
        string optionText = "گزینه جدید";
        int displayOrder = 0;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => question.AddOption(optionText, displayOrder));
    }
    
    [Fact]
    public void AddOption_For_FileUpload_Question_Throws_InvalidOperationException()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.FileUpload, 0);
        string optionText = "گزینه جدید";
        int displayOrder = 0;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => question.AddOption(optionText, displayOrder));
    }
    
    [Fact]
    public void RemoveOption_Removes_Option_And_Updates_ModifiedAt()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        question.AddOption("گزینه 1", 0);
        question.AddOption("گزینه 2", 1);
        Assert.Equal(2, question.Options.Count);
        
        // Get the ID of the first option
        var optionToRemove = question.Options.First();
        var optionId = optionToRemove.Id;
        
        // Reset ModifiedAt for clean testing
        typeof(SurveyQuestion).GetProperty("ModifiedAt")?.SetValue(question, null);
        Assert.Null(question.ModifiedAt);
        
        // Act
        question.RemoveOption(optionId);

        // Assert
        Assert.Single(question.Options);
        Assert.DoesNotContain(question.Options, o => o.Id == optionId);
        Assert.NotNull(question.ModifiedAt);
    }
    
    [Fact]
    public void RemoveOption_With_NonExistent_Id_Does_Nothing()
    {
        // Arrange
        var question = new SurveyQuestion(_survey, "سوال اولیه", QuestionType.SingleChoice, 0);
        question.AddOption("گزینه 1", 0);
        Assert.Single(question.Options);
        
        // Reset ModifiedAt for clean testing
        typeof(SurveyQuestion).GetProperty("ModifiedAt")?.SetValue(question, null);
        Assert.Null(question.ModifiedAt);
        
        // Act
        question.RemoveOption(Guid.NewGuid()); // Non-existent ID

        // Assert
        Assert.Single(question.Options); // Still has the one option
        Assert.Null(question.ModifiedAt); // No change was made
    }
}
