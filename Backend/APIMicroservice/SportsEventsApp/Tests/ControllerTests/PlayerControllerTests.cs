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
    public class PlayerControllerTests
    {
        private readonly Mock<IPlayerService> _playerServiceMock;
        private readonly PlayerController _playerController;

        public PlayerControllerTests()
        {
            _playerServiceMock = new Mock<IPlayerService>();
            _playerController = new PlayerController(_playerServiceMock.Object);
        }

        [Fact]
        public async Task GetPlayerInfo_ReturnsPlayer()
        {
            // Arrange
            int playerId = 1;
            var expectedPlayer = new PlayerDTO
            {
                Id = playerId,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Nationality = "English",
                Position = "Forward",
                ShirtNumber = 10,
                CurrentTeamId = 1,
                CurrentTeamName = "Team A",
                contractStart = new DateOnly(2020, 1, 1),
                contractEnd = new DateOnly(2025, 1, 1),
                MatchesOnPitch = 20,
                Starting = 18,
                MinutesPlayed = 1600,
                Goals = 10,
                OwnGoals = 1,
                Assists = 5,
                Penalties = 2,
                SubbedOut = 5,
                SubbedIn = 2,
                YellowCards = 3,
                YellowRedCards = 0,
                RedCards = 0
            };

            _playerServiceMock.Setup(service => service.GetPlayerInfo(playerId))
                              .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _playerController.GetPlayerInfo(playerId);

            // Assert
            var actionResult = Assert.IsType<PlayerDTO>(result);
            Assert.Equal(expectedPlayer, actionResult);
        }

        [Fact]
        public async Task GetPlayerInfo_ReturnsNull_WhenPlayerDoesNotExist()
        {
            // Arrange
            int playerId = 99;
            _playerServiceMock.Setup(service => service.GetPlayerInfo(playerId))
                              .ReturnsAsync((PlayerDTO)null);

            // Act
            var result = await _playerController.GetPlayerInfo(playerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTopScorersFromComp_ReturnsListOfTopScorers()
        {
            // Arrange
            int competitionId = 1;
            var expectedTopScorers = new List<TopScorerDTO>
            {
                new TopScorerDTO
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Nationality = "English",
                    Position = "Forward",
                    ShirtNumber = 10,
                    CurrentTeamId = 1,
                    CurrentTeamName = "Team A",
                    CurrentTeamCrest = "crest_a.png",
                    PlayedMatches = 20,
                    Goals = 10,
                    Assists = 5,
                    Penalties = 2
                },
                new TopScorerDTO
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Nationality = "French",
                    Position = "Midfielder",
                    ShirtNumber = 8,
                    CurrentTeamId = 2,
                    CurrentTeamName = "Team B",
                    CurrentTeamCrest = "crest_b.png",
                    PlayedMatches = 18,
                    Goals = 8,
                    Assists = 7,
                    Penalties = 1
                }
            };

            _playerServiceMock.Setup(service => service.TopScorersFromCompId(competitionId))
                              .ReturnsAsync(expectedTopScorers);

            // Act
            var result = await _playerController.GetTopScorersFromComp(competitionId);

            // Assert
            var actionResult = Assert.IsType<List<TopScorerDTO>>(result);
            Assert.Equal(expectedTopScorers.Count, actionResult.Count);
            Assert.Equal(expectedTopScorers, actionResult);
        }

        [Fact]
        public async Task GetTopScorersFromComp_ReturnsEmptyList_WhenNoTopScorersExist()
        {
            // Arrange
            int competitionId = 99;
            var expectedTopScorers = new List<TopScorerDTO>();

            _playerServiceMock.Setup(service => service.TopScorersFromCompId(competitionId))
                              .ReturnsAsync(expectedTopScorers);

            // Act
            var result = await _playerController.GetTopScorersFromComp(competitionId);

            // Assert
            var actionResult = Assert.IsType<List<TopScorerDTO>>(result);
            Assert.Empty(actionResult);
        }
    }
}
