using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Moq;
using Repository.Interface;
using Service.Implementation;
using Xunit;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepo> _playerRepoMock;
    private readonly PlayerService _playerService;

    public PlayerServiceTests()
    {
        _playerRepoMock = new Mock<IPlayerRepo>();
        _playerService = new PlayerService(_playerRepoMock.Object);
    }

    [Fact]
    public async Task GetPlayerInfo_ReturnsPlayer()
    {
        // Arrange
        var player = new PlayerDTO { Id = 1, FirstName = "John", LastName = "Doe" };
        _playerRepoMock.Setup(repo => repo.GetPlayer(1)).ReturnsAsync(player);

        // Act
        var result = await _playerService.GetPlayerInfo(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetPlayerInfo_ReturnsNull_WhenPlayerNotFound()
    {
        // Arrange
        _playerRepoMock.Setup(repo => repo.GetPlayer(1)).ReturnsAsync((PlayerDTO)null);
        
        // Act
        var result = await _playerService.GetPlayerInfo(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TopScorersFromCompId_ReturnsTopScorers()
    {
        // Arrange
        var topScorers = new List<TopScorerDTO> { new TopScorerDTO { Id = 1, FirstName = "John", LastName = "Doe", Goals = 10 } };
        _playerRepoMock.Setup(repo => repo.TopScorersFromCompId(100)).ReturnsAsync(topScorers);

        // Act
        var result = await _playerService.TopScorersFromCompId(100);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task TopScorersFromCompId_ReturnsEmptyList_WhenNoScorersFound()
    {
        // Arrange
        _playerRepoMock.Setup(repo => repo.TopScorersFromCompId(100)).ReturnsAsync(new List<TopScorerDTO>());

        // Act
        var result = await _playerService.TopScorersFromCompId(100);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
