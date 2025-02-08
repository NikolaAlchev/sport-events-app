using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Moq;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Xunit;

public class CompetitionsServiceTests
{
    private readonly Mock<ICompetitionsRepo> _competitionsRepoMock;
    private readonly CompetitionsService _competitionsService;

    public CompetitionsServiceTests()
    {
        _competitionsRepoMock = new Mock<ICompetitionsRepo>();
        _competitionsService = new CompetitionsService(_competitionsRepoMock.Object);
    }

    [Fact]
    public async Task GetAllCompetitions_ReturnsListOfCompetitions()
    {
        // Arrange
        var expectedCompetitions = new List<CompetitionDTO>
        {
            new CompetitionDTO
            {
                Id = 1,
                Country = "England",
                CountryFlag = "flag_england.png",
                Name = "Premier League",
                Emblem = "emblem_pl.png",
                Type = "League",
                CurrentSeasonId = 2023,
                CurrentSeasonStartDate = new DateOnly(2023, 8, 1),
                CurrentSeasonEndDate = new DateOnly(2024, 5, 31),
                CurrentMatchDay = 10
            },
            new CompetitionDTO
            {
                Id = 2,
                Country = "Spain",
                CountryFlag = "flag_spain.png",
                Name = "La Liga",
                Emblem = "emblem_ll.png",
                Type = "League",
                CurrentSeasonId = 2023,
                CurrentSeasonStartDate = new DateOnly(2023, 8, 15),
                CurrentSeasonEndDate = new DateOnly(2024, 5, 20),
                CurrentMatchDay = 8
            }
        };

        _competitionsRepoMock.Setup(repo => repo.getAllCompetitions())
                             .ReturnsAsync(expectedCompetitions);

        // Act
        var result = await _competitionsService.getAllCompetitions();

        // Assert
        var actionResult = Assert.IsType<List<CompetitionDTO>>(result);
        Assert.Equal(expectedCompetitions.Count, actionResult.Count);
        Assert.Equal(expectedCompetitions, actionResult);
    }

    [Fact]
    public async Task GetAllCompetitions_ReturnsEmptyList_WhenNoCompetitionsExist()
    {
        // Arrange
        var expectedCompetitions = new List<CompetitionDTO>();

        _competitionsRepoMock.Setup(repo => repo.getAllCompetitions())
                             .ReturnsAsync(expectedCompetitions);

        // Act
        var result = await _competitionsService.getAllCompetitions();

        // Assert
        var actionResult = Assert.IsType<List<CompetitionDTO>>(result);
        Assert.Empty(actionResult);
    }

    [Fact]
    public async Task GetTeamsByCompId_ReturnsListOfTeams()
    {
        // Arrange
        int competitionId = 1;
        var expectedTeams = new List<TeamDTO>
        {
            new TeamDTO
            {
                Id = 1,
                Name = "Team A",
                Crest = "crest_a.png",
                CurrentCompetitionId = competitionId
            },
            new TeamDTO
            {
                Id = 2,
                Name = "Team B",
                Crest = "crest_b.png",
                CurrentCompetitionId = competitionId
            }
        };

        _competitionsRepoMock.Setup(repo => repo.getAllTeamsFromCompetition(competitionId))
                             .ReturnsAsync(expectedTeams);

        // Act
        var result = await _competitionsService.getTeamsByCompId(competitionId);

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

        _competitionsRepoMock.Setup(repo => repo.getAllTeamsFromCompetition(competitionId))
                             .ReturnsAsync(expectedTeams);

        // Act
        var result = await _competitionsService.getTeamsByCompId(competitionId);

        // Assert
        var actionResult = Assert.IsType<List<TeamDTO>>(result);
        Assert.Empty(actionResult);
    }
}
