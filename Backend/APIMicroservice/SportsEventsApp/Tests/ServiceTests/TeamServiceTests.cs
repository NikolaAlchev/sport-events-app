using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Service.Implementation;
using Service.Interface;
using Domain.DTO;
using Repository.Interface;

namespace Service.Tests
{
    public class TeamServiceTests
    {
        private readonly Mock<ITeamRepo> _mockTeamRepo;
        private readonly ITeamService _teamService;

        public TeamServiceTests()
        {
            _mockTeamRepo = new Mock<ITeamRepo>();
            _teamService = new TeamService(_mockTeamRepo.Object);
        }

        [Fact]
        public async Task GetSquadFromTeamId_ReturnsPlayers()
        {
            // Arrange
            var players = new List<BasicPlayerInfoDTO> { new BasicPlayerInfoDTO { Id = 1, Name = "Player A", Position = "Midfielder" } };
            _mockTeamRepo.Setup(repo => repo.GetSquadFromTeamId(1)).ReturnsAsync(players);

            // Act
            var result = await _teamService.GetSquadFromTeamId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetSquadFromTeamId_ReturnsEmptyList_WhenNoPlayersFound()
        {
            // Arrange
            _mockTeamRepo.Setup(repo => repo.GetSquadFromTeamId(1)).ReturnsAsync(new List<BasicPlayerInfoDTO>());

            // Act
            var result = await _teamService.GetSquadFromTeamId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTeamByCompetitionIdAndName_ReturnsTeam()
        {
            // Arrange
            var team = new TeamPositionDTO { TeamId = 1, TeamName = "Team A", Position = 1 };
            _mockTeamRepo.Setup(repo => repo.GetTeamByCompetitionIdAndName(10, "Team A")).ReturnsAsync(team);

            // Act
            var result = await _teamService.GetTeamByCompetitionIdAndName(10, "Team A");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.TeamName);
        }

        [Fact]
        public async Task GetTeamByCompetitionIdAndName_ReturnsNull_WhenNoTeamFound()
        {
            // Arrange
            _mockTeamRepo.Setup(repo => repo.GetTeamByCompetitionIdAndName(10, "Team A")).ReturnsAsync((TeamPositionDTO)null);

            // Act
            var result = await _teamService.GetTeamByCompetitionIdAndName(10, "Team A");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTeamById_ReturnsTeamInfo()
        {
            // Arrange
            var teamInfo = new TeamInfoDTO { Name = "Team A", Venue = "Stadium A", Crest = "crest_url" };
            _mockTeamRepo.Setup(repo => repo.GetTeamById(1)).ReturnsAsync(teamInfo);

            // Act
            var result = await _teamService.GetTeamById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.Name);
        }

        [Fact]
        public async Task GetTeamById_ReturnsNull_WhenTeamNotFound()
        {
            // Arrange
            _mockTeamRepo.Setup(repo => repo.GetTeamById(1)).ReturnsAsync((TeamInfoDTO)null);

            // Act
            var result = await _teamService.GetTeamById(1);

            // Assert
            Assert.Null(result);
        }
    }
}
