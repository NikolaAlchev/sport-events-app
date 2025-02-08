using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Moq;
using Repository.Interface;
using Service.Implementation;
using Xunit;

public class MatchesServiceTests
{
    private readonly Mock<IMatchesRepo> _matchesRepoMock;
    private readonly Mock<ICompetitionsRepo> _competitionsRepoMock;
    private readonly MatchesService _matchesService;

    public MatchesServiceTests()
    {
        _matchesRepoMock = new Mock<IMatchesRepo>();
        _competitionsRepoMock = new Mock<ICompetitionsRepo>();
        _matchesService = new MatchesService(_matchesRepoMock.Object, _competitionsRepoMock.Object);
    }

    [Fact]
    public async Task GetAllMatchesFromDate_ReturnsMatches()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 8);
        var matches = new List<MatchDTO>
        {
            new MatchDTO { Id = 1, HomeTeamName = "Team A", AwayTeamName = "Team B" }
        };
        _matchesRepoMock.Setup(repo => repo.getAllMatchesFromDate(date)).ReturnsAsync(matches);

        // Act
        var result = await _matchesService.getAllMatchesFromDate(date);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Team A", result[0].HomeTeamName);
    }

    [Fact]
    public async Task GetAllMatchesFromDate_ReturnsEmptyList()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 8);
        _matchesRepoMock.Setup(repo => repo.getAllMatchesFromDate(date)).ReturnsAsync(new List<MatchDTO>());

        // Act
        var result = await _matchesService.getAllMatchesFromDate(date);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMatchById_ReturnsMatch()
    {
        // Arrange
        var match = new MatchDTO { Id = 1, HomeTeamName = "Team A", AwayTeamName = "Team B" };
        _matchesRepoMock.Setup(repo => repo.getMatchById(1)).ReturnsAsync(match);

        // Act
        var result = await _matchesService.getMatchById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Team A", result.HomeTeamName);
    }

    [Fact]
    public async Task GetMatchById_ReturnsNull()
    {
        // Arrange
        _matchesRepoMock.Setup(repo => repo.getMatchById(1)).ReturnsAsync((MatchDTO)null);

        // Act
        var result = await _matchesService.getMatchById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStandingsTableForComp_ReturnsStandings()
    {
        // Arrange
        var standings = new StandingsTableDTO
        {
            CompetitionId = 1,
            CompetitionName = "Premier League",
            teamStandings = new List<TeamPositionDTO>()
        };
        _matchesRepoMock.Setup(repo => repo.getStandingsTable(1)).ReturnsAsync(standings);

        // Act
        var result = await _matchesService.getStandingsTableForComp(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CompetitionId);
        Assert.Equal("Premier League", result.CompetitionName);
    }

    [Fact]
    public async Task GetStandingsTableForComp_ReturnsNull()
    {
        // Arrange
        _matchesRepoMock.Setup(repo => repo.getStandingsTable(1)).ReturnsAsync((StandingsTableDTO)null);

        // Act
        var result = await _matchesService.getStandingsTableForComp(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStandingsTableForComp_ReturnsStandingsForBothTeams()
    {
        // Arrange
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(1)).ReturnsAsync(10);
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(2)).ReturnsAsync(20);
        _matchesRepoMock.Setup(repo => repo.getStandingsTable(10)).ReturnsAsync(new StandingsTableDTO { CompetitionId = 10 });
        _matchesRepoMock.Setup(repo => repo.getStandingsTable(20)).ReturnsAsync(new StandingsTableDTO { CompetitionId = 20 });
        
        // Act 
        var result = await _matchesService.getStandingsTableForComp(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetStandingsTableForComp_ThrowsException_WhenHomeTeamHasNoLeague()
    {
        // Arrange
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(1)).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _matchesService.getStandingsTableForComp(1, 2));
    }

    [Fact]
    public async Task GetStandingsTableForComp_ThrowsException_WhenAwayTeamHasNoLeague()
    {
        // Arrange
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(1)).ReturnsAsync(10);
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(2)).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _matchesService.getStandingsTableForComp(1, 2));
    }

    [Fact]
    public async Task GetStandingsTableForComp_ReturnsSingleLeague_WhenBothTeamsAreInSameLeague()
    {
        // Arrange
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(1)).ReturnsAsync(10);
        _competitionsRepoMock.Setup(repo => repo.standingsForLeagueFromTeamId(2)).ReturnsAsync(10);
        _matchesRepoMock.Setup(repo => repo.getStandingsTable(10)).ReturnsAsync(new StandingsTableDTO { CompetitionId = 10 });

        // Act
        var result = await _matchesService.getStandingsTableForComp(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(10, result[0].CompetitionId);
    }

    [Fact]
    public async Task GetHead2Head_ReturnsMatches()
    {
        // Arrange
        var matches = new List<MatchDTO>
        {
            new MatchDTO { Id = 1, HomeTeamName = "Team A", AwayTeamName = "Team B" }
        };
        _matchesRepoMock.Setup(repo => repo.getHead2Head(1)).ReturnsAsync(matches);

        // Act
        var result = await _matchesService.getHead2Head(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Team A", result[0].HomeTeamName);
    }

    [Fact]
    public async Task GetHead2Head_ReturnsEmptyList()
    {
        // Arrange
        _matchesRepoMock.Setup(repo => repo.getHead2Head(1)).ReturnsAsync(new List<MatchDTO>());

        // Act
        var result = await _matchesService.getHead2Head(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
