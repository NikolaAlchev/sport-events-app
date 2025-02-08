using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Controllers;
using Domain.DTO;
using Xunit;

namespace Web.Tests
{
    public class TeamControllerTests
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly TeamController _teamController;

        public TeamControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamController = new TeamController(_teamServiceMock.Object);
        }

        [Fact]
        public async Task GetTeamById_ReturnsTeamInfo()
        {
            // Arrange
            int teamId = 1;
            var expectedTeamInfo = new TeamInfoDTO
            {
                Name = "Team A",
                Venue = "Stadium A",
                Crest = "crest_a.png",
                PastMatches = new List<MatchDTO>
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
                },
                FutureMatches = new List<MatchDTO>
                {
                    new MatchDTO
                    {
                        Id = 2,
                        UtcDate = new DateTime(2023, 10, 15, 15, 0, 0),
                        Status = "SCHEDULED",
                        HomeTeamId = 1,
                        HomeTeamName = "Team A",
                        AwayTeamId = 3,
                        AwayTeamName = "Team C",
                        HomeTeamScore = 0,
                        AwayTeamScore = 0,
                        CompetitionName = "Premier League",
                        CompetitionId = 1,
                        HomeTeamCrest = "crest_a.png",
                        AwayTeamCrest = "crest_c.png",
                        Venue = "Stadium A",
                        LeagueEmblem = "emblem.png",
                        Referees = new List<RefereeDTO>()
                    }
                },
                Squad = new List<BasicPlayerInfoDTO>
                {
                    new BasicPlayerInfoDTO
                    {
                        Id = 1,
                        Name = "John Doe",
                        Position = "Forward",
                        Nationality = "English"
                    },
                    new BasicPlayerInfoDTO
                    {
                        Id = 2,
                        Name = "Jane Smith",
                        Position = "Midfielder",
                        Nationality = "French"
                    }
                }
            };

            _teamServiceMock.Setup(service => service.GetTeamById(teamId))
                            .ReturnsAsync(expectedTeamInfo);

            // Act
            var result = await _teamController.GetTeamById(teamId);

            // Assert
            var actionResult = Assert.IsType<TeamInfoDTO>(result);
            Assert.Equal(expectedTeamInfo, actionResult);
        }

        [Fact]
        public async Task GetTeamById_ReturnsNull_WhenTeamDoesNotExist()
        {
            // Arrange
            int teamId = 99;
            _teamServiceMock.Setup(service => service.GetTeamById(teamId))
                            .ReturnsAsync((TeamInfoDTO)null);

            // Act
            var result = await _teamController.GetTeamById(teamId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSquadOfTeamByTeamId_ReturnsSquad()
        {
            // Arrange
            int teamId = 1;
            var expectedSquad = new List<BasicPlayerInfoDTO>
            {
                new BasicPlayerInfoDTO
                {
                    Id = 1,
                    Name = "John Doe",
                    Position = "Forward",
                    Nationality = "English"
                },
                new BasicPlayerInfoDTO
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Position = "Midfielder",
                    Nationality = "French"
                }
            };

            _teamServiceMock.Setup(service => service.GetSquadFromTeamId(teamId))
                            .ReturnsAsync(expectedSquad);

            // Act
            var result = await _teamController.GetSquadOfTeamByTeamId(teamId);

            // Assert
            var actionResult = Assert.IsType<List<BasicPlayerInfoDTO>>(result);
            Assert.Equal(expectedSquad.Count, actionResult.Count);
            Assert.Equal(expectedSquad, actionResult);
        }

        [Fact]
        public async Task GetSquadOfTeamByTeamId_ReturnsEmptyList_WhenNoSquadExists()
        {
            // Arrange
            int teamId = 99;
            var expectedSquad = new List<BasicPlayerInfoDTO>();

            _teamServiceMock.Setup(service => service.GetSquadFromTeamId(teamId))
                            .ReturnsAsync(expectedSquad);

            // Act
            var result = await _teamController.GetSquadOfTeamByTeamId(teamId);

            // Assert
            var actionResult = Assert.IsType<List<BasicPlayerInfoDTO>>(result);
            Assert.Empty(actionResult);
        }
    }
}
