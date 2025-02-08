using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Controllers;
using Domain.DTO;
using Xunit;

namespace Web.Tests
{
    public class MatchesControllerTests
    {
        private readonly Mock<IMatchesService> _matchesServiceMock;
        private readonly MatchesController _matchesController;

        public MatchesControllerTests()
        {
            _matchesServiceMock = new Mock<IMatchesService>();
            _matchesController = new MatchesController(_matchesServiceMock.Object);
        }

        [Fact]
        public async Task GetAllMatchesOnDate_ReturnsListOfMatches()
        {
            // Arrange
            var fromDate = new DateOnly(2023, 10, 1);
            var expectedMatches = new List<MatchDTO> 
            {
                new MatchDTO
                {
                    Id = 1,
                    UtcDate = new DateTime(2023, 10, 1, 15, 0, 0),
                    Status = "FINISHED",
                    HomeTeamId = 1,
                    HomeTeamName = "Team A",
                    AwayTeamId = 2,
                    AwayTeamName = "Team B",
                    HomeTeamScore = 2,
                    AwayTeamScore = 1,
                    CompetitionName = "Premier League",
                    CompetitionId = 1,
                    HomeTeamCrest = "crest_a.png",
                    AwayTeamCrest = "crest_b.png",
                    Venue = "Stadium A",
                    LeagueEmblem = "emblem.png",
                    Referees = new List<RefereeDTO>()
                }
            };

            _matchesServiceMock.Setup(service => service.getAllMatchesFromDate(fromDate))
                               .ReturnsAsync(expectedMatches);

            // Act
            var result = await _matchesController.getAllMatchesOnDate(fromDate);

            // Assert
            var actionResult = Assert.IsType<List<MatchDTO>>(result);
            Assert.Equal(expectedMatches.Count, actionResult.Count);
            Assert.Equal(expectedMatches, actionResult);
        }

        [Fact]
        public async Task GetMatchById_ReturnsMatch()
        {
            // Arrange
            int matchId = 1;
            var expectedMatch = new MatchDTO
            {
                Id = matchId,
                UtcDate = new DateTime(2023, 10, 1, 15, 0, 0),
                Status = "FINISHED",
                HomeTeamId = 1,
                HomeTeamName = "Team A",
                AwayTeamId = 2,
                AwayTeamName = "Team B",
                HomeTeamScore = 2,
                AwayTeamScore = 1,
                CompetitionName = "Premier League",
                CompetitionId = 1,
                HomeTeamCrest = "crest_a.png",
                AwayTeamCrest = "crest_b.png",
                Venue = "Stadium A",
                LeagueEmblem = "emblem.png",
                Referees = new List<RefereeDTO>()
            };

            _matchesServiceMock.Setup(service => service.getMatchById(matchId))
                               .ReturnsAsync(expectedMatch);

            // Act
            var result = await _matchesController.getMatchById(matchId);

            // Assert
            var actionResult = Assert.IsType<MatchDTO>(result);
            Assert.Equal(expectedMatch, actionResult);
        }

        [Fact]
        public async Task GetMatchById_ReturnsNull_WhenMatchDoesNotExist()
        {
            // Arrange
            int matchId = 99;
            _matchesServiceMock.Setup(service => service.getMatchById(matchId))
                               .ReturnsAsync((MatchDTO)null);

            // Act
            var result = await _matchesController.getMatchById(matchId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStandings_ReturnsStandingsTable()
        {
            // Arrange
            int homeTeam = 1;
            int awayTeam = 2;
            var expectedStandings = new List<StandingsTableDTO>
            {
                new StandingsTableDTO
                {
                    CompetitionId = 1,
                    CompetitionName = "Premier League",
                    teamStandings = new List<TeamPositionDTO>
                    {
                        new TeamPositionDTO { TeamId = homeTeam, TeamName = "Team A", Points = 10 },
                        new TeamPositionDTO { TeamId = awayTeam, TeamName = "Team B", Points = 8 }
                    }
                }
            };

            _matchesServiceMock.Setup(service => service.getStandingsTableForComp(homeTeam, awayTeam))
                               .ReturnsAsync(expectedStandings);

            // Act
            var result = await _matchesController.getStandings(homeTeam, awayTeam);

            // Assert
            var actionResult = Assert.IsType<List<StandingsTableDTO>>(result);
            Assert.Equal(expectedStandings.Count, actionResult.Count);
            Assert.Equal(expectedStandings, actionResult);
        }

        [Fact]
        public async Task GetStandings_ReturnsEmptyList_WhenNoStandingsExist()
        {
            // Arrange
            int homeTeam = 99;
            int awayTeam = 100;
            var expectedStandings = new List<StandingsTableDTO>();

            _matchesServiceMock.Setup(service => service.getStandingsTableForComp(homeTeam, awayTeam))
                               .ReturnsAsync(expectedStandings);

            // Act
            var result = await _matchesController.getStandings(homeTeam, awayTeam);

            // Assert
            var actionResult = Assert.IsType<List<StandingsTableDTO>>(result);
            Assert.Empty(actionResult);
        }

        [Fact]
        public async Task GetHead2Head_ReturnsListOfMatches()
        {
            // Arrange
            int teamId = 1;
            var expectedMatches = new List<MatchDTO>
            {
                new MatchDTO
                {
                    Id = 1,
                    UtcDate = new DateTime(2023, 10, 1, 15, 0, 0),
                    Status = "FINISHED",
                    HomeTeamId = 1,
                    HomeTeamName = "Team A",
                    AwayTeamId = 2,
                    AwayTeamName = "Team B",
                    HomeTeamScore = 2,
                    AwayTeamScore = 1,
                    CompetitionName = "Premier League",
                    CompetitionId = 1,
                    HomeTeamCrest = "crest_a.png",
                    AwayTeamCrest = "crest_b.png",
                    Venue = "Stadium A",
                    LeagueEmblem = "emblem.png",
                    Referees = new List<RefereeDTO>()
                }
            };

            _matchesServiceMock.Setup(service => service.getHead2Head(teamId))
                               .ReturnsAsync(expectedMatches);

            // Act
            var result = await _matchesController.getHead2Head(teamId);

            // Assert
            var actionResult = Assert.IsType<List<MatchDTO>>(result);
            Assert.Equal(expectedMatches.Count, actionResult.Count);
            Assert.Equal(expectedMatches, actionResult);
        }

        [Fact]
        public async Task GetHead2Head_ReturnsEmptyList_WhenNoMatchesExist()
        {
            // Arrange
            int teamId = 99;
            var expectedMatches = new List<MatchDTO>();

            _matchesServiceMock.Setup(service => service.getHead2Head(teamId))
                               .ReturnsAsync(expectedMatches);

            // Act
            var result = await _matchesController.getHead2Head(teamId);

            // Assert
            var actionResult = Assert.IsType<List<MatchDTO>>(result);
            Assert.Empty(actionResult);
        }
    }
}
