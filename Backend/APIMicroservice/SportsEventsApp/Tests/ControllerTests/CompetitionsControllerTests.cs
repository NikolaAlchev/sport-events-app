using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Controllers;
using Xunit;
using Domain.DTO;

public class CompetitionsControllerTests
{
    private readonly Mock<ICompetitionsService> _competitionsServiceMock;
    private readonly CompetitionsController _competitionsController;

    public CompetitionsControllerTests()
    {
        _competitionsServiceMock = new Mock<ICompetitionsService>();
        _competitionsController = new CompetitionsController(_competitionsServiceMock.Object);
    }

    [Fact]
    public async Task GetAllCompetitions_ReturnsListOfCompetitions()
    {
        // Arrange
        var expectedCompetitions = new List<CompetitionDTO>
        {
            new CompetitionDTO { Id = 1, Name = "Competition 1" },
            new CompetitionDTO { Id = 2, Name = "Competition 2" }
        };

        _competitionsServiceMock.Setup(service => service.getAllCompetitions())
                        .ReturnsAsync(expectedCompetitions);

        // Act
        var result = await _competitionsController.getAllCompetitions();

        // Assert
        var actionResult = Assert.IsType<List<CompetitionDTO>>(result);
        Assert.Equal(expectedCompetitions.Count, actionResult.Count);
        Assert.Equal(expectedCompetitions, actionResult);
    }

    [Fact]
    public async Task GetTeamsByCompId_ReturnsListOfTeams()
    {
        // Arrange
        int competitionId = 1;
        var expectedTeams = new List<TeamDTO>
        {
            new TeamDTO { Id = 1, Name = "Team A" },
            new TeamDTO { Id = 2, Name = "Team B" }
        };

        _competitionsServiceMock.Setup(service => service.getTeamsByCompId(competitionId))
                        .ReturnsAsync(expectedTeams);

        // Act
        var result = await _competitionsController.getTeamsByCompId(competitionId);

        // Assert
        var actionResult = Assert.IsType<List<TeamDTO>>(result);
        Assert.Equal(expectedTeams.Count, actionResult.Count);
        Assert.Equal(expectedTeams, actionResult);
    }

    [Fact]
    public async Task GetTeamsByCompId_ReturnsEmptyList_WhenNoTeamsExist()
    {
        // Arrange
        int competitionId = 99;
        var expectedTeams = new List<TeamDTO>();

        _competitionsServiceMock.Setup(service => service.getTeamsByCompId(competitionId))
                        .ReturnsAsync(expectedTeams);

        // Act
        var result = await _competitionsController.getTeamsByCompId(competitionId);

        // Assert
        var actionResult = Assert.IsType<List<TeamDTO>>(result);
        Assert.Empty(actionResult);
    }
}
