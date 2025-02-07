using Moq;
using Xunit;
using EventsApp.Controllers;
using Repository.Interface;
using Domain.SecondAppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DTO;

public class QuestionsControllerTests
{
    private readonly Mock<IQuestionAppRepository> _mockQuestionRepository;
    private readonly QuestionsController _controller;

    public QuestionsControllerTests()
    {
        _mockQuestionRepository = new Mock<IQuestionAppRepository>();
        _controller = new QuestionsController(_mockQuestionRepository.Object);
    }

    [Fact]
    public void Index_ValidData_ReturnsUserQuestionsDTOList()
    {
        // Arrange
        var questions = new List<Questions>
        {
            new Questions { DatabaseId = Guid.NewGuid(), QuestionText = "Question 1", QuestionAnswer = "Answer 1" }
        };
        var databases = new List<Databases>
        {
            new Databases { Id = Guid.NewGuid(), OwnerId = "Owner1" }
        };
        var users = new List<Users>
        {
            new Users { Id = "Owner1", UserName = "User1" }
        };

        _mockQuestionRepository.Setup(r => r.getAllQuestions()).Returns(questions);
        _mockQuestionRepository.Setup(r => r.getAllDatabases()).Returns(databases);
        _mockQuestionRepository.Setup(r => r.getAllUsers()).Returns(users);

        // Act
        var result = _controller.Index();

        // Assert
        var userQuestions = Assert.IsType<List<UserQuestionsDTO>>(result);
        Assert.Single(userQuestions); // We have only one question in the test data
        Assert.Equal("User1", userQuestions[0].UserName);
        Assert.Equal("Question 1", userQuestions[0].QuestionText);
        Assert.Equal("Answer 1", userQuestions[0].QuestionAnswer);
    }

    [Fact]
    public void Index_TablesAreEmpty_ThrowsException()
    {
        // Arrange
        _mockQuestionRepository.Setup(r => r.getAllQuestions()).Returns(new List<Questions>());
        _mockQuestionRepository.Setup(r => r.getAllDatabases()).Returns(new List<Databases>());
        _mockQuestionRepository.Setup(r => r.getAllUsers()).Returns(new List<Users>());

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _controller.Index());
        Assert.Equal("Tables must not be empty", exception.Message);
    }

    [Fact]
    public void Index_QuestionsAreEmpty_ThrowsException()
    {
        // Arrange
        var databases = new List<Databases> { new Databases { Id = Guid.NewGuid(), OwnerId = "Owner1" } };
        var users = new List<Users> { new Users { Id = "Owner1", UserName = "User1" } };

        _mockQuestionRepository.Setup(r => r.getAllQuestions()).Returns(new List<Questions>());
        _mockQuestionRepository.Setup(r => r.getAllDatabases()).Returns(databases);
        _mockQuestionRepository.Setup(r => r.getAllUsers()).Returns(users);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _controller.Index());
        Assert.Equal("Tables must not be empty", exception.Message);
    }

    [Fact]
    public void Index_DatabasesAreEmpty_ThrowsException()
    {
        // Arrange
        var questions = new List<Questions>
        {
            new Questions { DatabaseId = Guid.NewGuid(), QuestionText = "Question 1", QuestionAnswer = "Answer 1" }
        };
        var users = new List<Users> { new Users { Id = "Owner1", UserName = "User1" } };

        _mockQuestionRepository.Setup(r => r.getAllQuestions()).Returns(questions);
        _mockQuestionRepository.Setup(r => r.getAllDatabases()).Returns(new List<Databases>());
        _mockQuestionRepository.Setup(r => r.getAllUsers()).Returns(users);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _controller.Index());
        Assert.Equal("Tables must not be empty", exception.Message);
    }

    [Fact]
    public void Index_UsersAreEmpty_ThrowsException()
    {
        // Arrange
        var questions = new List<Questions>
        {
            new Questions { DatabaseId = Guid.NewGuid(), QuestionText = "Question 1", QuestionAnswer = "Answer 1" }
        };
        var databases = new List<Databases> { new Databases { Id = Guid.NewGuid(), OwnerId = "Owner1" } };

        _mockQuestionRepository.Setup(r => r.getAllQuestions()).Returns(questions);
        _mockQuestionRepository.Setup(r => r.getAllDatabases()).Returns(databases);
        _mockQuestionRepository.Setup(r => r.getAllUsers()).Returns(new List<Users>());

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _controller.Index());
        Assert.Equal("Tables must not be empty", exception.Message);
    }
}
