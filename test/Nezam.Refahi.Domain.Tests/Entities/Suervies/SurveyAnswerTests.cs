using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Suervies;

public class SurveyAnswerTests
{
    private readonly Survey _survey;
    private readonly SurveyResponse _response;
    private readonly SurveyQuestion _singleChoiceQuestion;
    private readonly SurveyQuestion _openTextQuestion;
    private readonly SurveyQuestion _fileUploadQuestion;
    private readonly SurveyOptions _option;
    
    public SurveyAnswerTests()
    {
        // Create standard test objects
        var creator = new User("علی", "محمدی", "2741153671", "09123456789");
        var responder = new User("رضا", "احمدی", "0741153671", "09187654321");
        
        _survey = new Survey("نظرسنجی آزمایشی", "توضیحات نظرسنجی", SurveyMode.QnA, creator, DateTimeOffset.UtcNow);
        _response = new SurveyResponse(_survey, responder);
        
        _singleChoiceQuestion = new SurveyQuestion(_survey, "سوال چند گزینه‌ای", QuestionType.SingleChoice, 0);
        _option = new SurveyOptions(_singleChoiceQuestion, "گزینه اول", 0);
        
        _openTextQuestion = new SurveyQuestion(_survey, "سوال متنی", QuestionType.OpenText, 1);
        _fileUploadQuestion = new SurveyQuestion(_survey, "آپلود فایل", QuestionType.FileUpload, 2);
    }
    
    [Fact]
    public void SurveyAnswer_Creation_For_SingleChoice_Sets_Required_Properties()
    {
        // Act
        var answer = new SurveyAnswer(_response, _singleChoiceQuestion, _option);

        // Assert
        Assert.Equal(_response.Id, answer.ResponseId);
        Assert.Same(_response, answer.Response);
        Assert.Equal(_singleChoiceQuestion.Id, answer.QuestionId);
        Assert.Same(_singleChoiceQuestion, answer.Question);
        Assert.Equal(_option.Id, answer.OptionId);
        Assert.Same(_option, answer.Option);
        Assert.Null(answer.TextAnswer);
        Assert.Null(answer.FilePath);
        Assert.NotEqual(Guid.Empty, answer.Id);
        Assert.NotEqual(default, answer.CreatedAt);
        Assert.Null(answer.ModifiedAt);
    }
    
    [Fact]
    public void SurveyAnswer_Creation_For_OpenText_Sets_Required_Properties()
    {
        // Arrange
        string textAnswer = "پاسخ متنی به سوال";
        
        // Act
        var answer = new SurveyAnswer(_response, _openTextQuestion, null, textAnswer);

        // Assert
        Assert.Equal(_response.Id, answer.ResponseId);
        Assert.Same(_response, answer.Response);
        Assert.Equal(_openTextQuestion.Id, answer.QuestionId);
        Assert.Same(_openTextQuestion, answer.Question);
        Assert.Null(answer.OptionId);
        Assert.Null(answer.Option);
        Assert.Equal(textAnswer, answer.TextAnswer);
        Assert.Null(answer.FilePath);
    }
    
    [Fact]
    public void SurveyAnswer_Creation_For_FileUpload_Sets_Required_Properties()
    {
        // Arrange
        string filePath = "/uploads/test-file.pdf";
        
        // Act
        var answer = new SurveyAnswer(_response, _fileUploadQuestion, null, null, filePath);

        // Assert
        Assert.Equal(_response.Id, answer.ResponseId);
        Assert.Same(_response, answer.Response);
        Assert.Equal(_fileUploadQuestion.Id, answer.QuestionId);
        Assert.Same(_fileUploadQuestion, answer.Question);
        Assert.Null(answer.OptionId);
        Assert.Null(answer.Option);
        Assert.Null(answer.TextAnswer);
        Assert.Equal(filePath, answer.FilePath);
    }
    
    [Fact]
    public void SurveyAnswer_Creation_With_Null_Response_Throws_ArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new SurveyAnswer(null!, _singleChoiceQuestion, _option));
    }
    
    [Fact]
    public void SurveyAnswer_Creation_With_Null_Question_Throws_ArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new SurveyAnswer(_response, null!, _option));
    }
    
    [Fact]
    public void SurveyAnswer_For_SingleChoice_Without_Option_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _singleChoiceQuestion, null));
    }
    
    [Fact]
    public void SurveyAnswer_For_SingleChoice_With_TextAnswer_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _singleChoiceQuestion, _option, "Invalid text"));
    }
    
    [Fact]
    public void SurveyAnswer_For_SingleChoice_With_FilePath_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _singleChoiceQuestion, _option, null, "invalid/path.txt"));
    }
    
    [Fact]
    public void SurveyAnswer_For_OpenText_With_Option_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _openTextQuestion, _option, "Text answer"));
    }
    
    [Fact]
    public void SurveyAnswer_For_OpenText_Without_TextAnswer_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _openTextQuestion, null));
    }
    
    [Fact]
    public void SurveyAnswer_For_OpenText_With_FilePath_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _openTextQuestion, null, "Text answer", "invalid/path.txt"));
    }
    
    [Fact]
    public void SurveyAnswer_For_FileUpload_With_Option_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _fileUploadQuestion, _option, null, "/uploads/file.pdf"));
    }
    
    [Fact]
    public void SurveyAnswer_For_FileUpload_Without_FilePath_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _fileUploadQuestion, null));
    }
    
    [Fact]
    public void SurveyAnswer_For_FileUpload_With_TextAnswer_Throws_ArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new SurveyAnswer(_response, _fileUploadQuestion, null, "Invalid text", "/uploads/file.pdf"));
    }
    
    [Fact]
    public void UpdateTextAnswer_For_OpenText_Updates_TextAnswer_And_ModifiedAt()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _openTextQuestion, null, "پاسخ اولیه");
        string newText = "پاسخ به‌روز شده";

        // Act
        answer.UpdateTextAnswer(newText);

        // Assert
        Assert.Equal(newText, answer.TextAnswer);
        Assert.NotNull(answer.ModifiedAt);
    }
    
    [Fact]
    public void UpdateTextAnswer_For_NonOpenText_Throws_InvalidOperationException()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _singleChoiceQuestion, _option);
        string newText = "پاسخ نامعتبر";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => answer.UpdateTextAnswer(newText));
    }
    
    [Fact]
    public void UpdateFileAnswer_For_FileUpload_Updates_FilePath_And_ModifiedAt()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _fileUploadQuestion, null, null, "/uploads/original.pdf");
        string newPath = "/uploads/updated.pdf";

        // Act
        answer.UpdateFileAnswer(newPath);

        // Assert
        Assert.Equal(newPath, answer.FilePath);
        Assert.NotNull(answer.ModifiedAt);
    }
    
    [Fact]
    public void UpdateFileAnswer_For_NonFileUpload_Throws_InvalidOperationException()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _singleChoiceQuestion, _option);
        string newPath = "/uploads/invalid.pdf";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => answer.UpdateFileAnswer(newPath));
    }
    
    [Fact]
    public void UpdateOptionAnswer_For_Choice_Updates_Option_And_ModifiedAt()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _singleChoiceQuestion, _option);
        var newOption = new SurveyOptions(_singleChoiceQuestion, "گزینه جدید", 1);

        // Act
        answer.UpdateOptionAnswer(newOption);

        // Assert
        Assert.Equal(newOption.Id, answer.OptionId);
        Assert.Same(newOption, answer.Option);
        Assert.NotNull(answer.ModifiedAt);
    }
    
    [Fact]
    public void UpdateOptionAnswer_For_NonChoice_Throws_InvalidOperationException()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _openTextQuestion, null, "پاسخ متنی");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => answer.UpdateOptionAnswer(_option));
    }
    
    [Fact]
    public void UpdateOptionAnswer_With_Null_Option_Throws_ArgumentNullException()
    {
        // Arrange
        var answer = new SurveyAnswer(_response, _singleChoiceQuestion, _option);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => answer.UpdateOptionAnswer(null!));
    }
}
