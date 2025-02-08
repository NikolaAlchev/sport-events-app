using ClosedXML.Excel;
using Domain.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Repository.Interface;
using Service.Implementation;

public class EventServiceTests
{
    private readonly Mock<IRepository<Event>> _mockEventRepository;
    private readonly Mock<IEventsRepository> _mockEventsRepository;
    private readonly Mock<IEventUserRepository> _mockEventUserRepository;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        // ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        _mockEventRepository = new Mock<IRepository<Event>>();
        _mockEventsRepository = new Mock<IEventsRepository>();
        _mockEventUserRepository = new Mock<IEventUserRepository>();
        _eventService = new EventService(
            _mockEventRepository.Object,
            _mockEventsRepository.Object,
            _mockEventUserRepository.Object
        );
    }

    [Fact]
    public void AddEvent_ShouldCallInsertOnRepository()
    {
        // Arrange
        var eventData = new Event { Id = Guid.NewGuid(), Title = "Test Event" };
        _mockEventRepository.Setup(repo => repo.Insert(It.IsAny<Event>())).Returns(eventData);

        // Act
        var result = _eventService.AddEvent(eventData);

        // Assert
        _mockEventRepository.Verify(repo => repo.Insert(It.IsAny<Event>()), Times.Once);
        result.Should().BeEquivalentTo(eventData);
    }

    [Fact]
    public void DeleteEvent_ShouldCallDeleteOnRepository()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventData = new Event { Id = eventId };
        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(eventData);
        _mockEventRepository.Setup(repo => repo.Delete(eventData)).Returns(eventData);

        // Act
        var result = _eventService.DeleteEvent(eventId);

        // Assert
        _mockEventRepository.Verify(repo => repo.Delete(It.IsAny<Event>()), Times.Once);
        result.Should().BeEquivalentTo(eventData);
    }

    [Fact]
    public void DeleteEvent_NonExistentId_ReturnsNull()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns((Event)null);

        // Act
        var result = _eventService.DeleteEvent(eventId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEvent_ValidId_ReturnsEvent()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var expectedEvent = new Event { Id = eventId, Title = "Test Event" };
        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(expectedEvent);

        // Act
        var result = _eventService.GetEvent(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEvent.Id, result.Id);
        Assert.Equal(expectedEvent.Title, result.Title);
    }

    [Fact]
    public void GetEvent_NullId_ReturnsNull()
    {
        // Act
        var result = _eventService.GetEvent(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEvent_NonExistentId_ReturnsNull()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns((Event)null);

        // Act
        var result = _eventService.GetEvent(eventId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllEvents()
    {
        // Arrange
        var eventData = new List<Event> { new Event { Id = Guid.NewGuid(), Title = "Test Event 1" } };
        _mockEventRepository.Setup(repo => repo.GetAll()).Returns(eventData.AsQueryable());

        // Act
        var result = _eventService.GetAll();

        // Assert
        result.Should().BeEquivalentTo(eventData);
    }

    [Fact]
    public void UpdateEvent_ValidEvent_ReturnsUpdatedEvent()
    {
        // Arrange
        var eventToUpdate = new Event { Id = Guid.NewGuid(), Title = "Updated Event" };
        _mockEventRepository.Setup(repo => repo.Update(eventToUpdate)).Returns(eventToUpdate);

        // Act
        var result = _eventService.UpdateEvent(eventToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventToUpdate.Id, result.Id);
        Assert.Equal(eventToUpdate.Title, result.Title);
    }

    [Fact]
    public void UpdateEvent_NullEvent_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _eventService.UpdateEvent(null));
    }

    [Fact]
    public void GetAllPaginated_ValidParameters_ReturnsEventsList()
    {
        // Arrange
        int offset = 0, limit = 2;
        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Title = "Event 1" },
            new Event { Id = Guid.NewGuid(), Title = "Event 2" }
        };
        _mockEventRepository.Setup(repo => repo.GetAllPaginated(offset, limit)).Returns(events);

        // Act
        var result = _eventService.GetAllPaginated(offset, limit);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(events[0].Id, result[0].Id);
        Assert.Equal(events[1].Id, result[1].Id);
    }

    [Fact]
    public void GetAllPaginated_NoEvents_ReturnsEmptyList()
    {
        // Arrange
        _mockEventRepository.Setup(repo => repo.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Event>());

        // Act
        var result = _eventService.GetAllPaginated(0, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllPaginated_WithFilters_ReturnsFilteredEvents()
    {
        // Arrange
        int offset = 0, limit = 2, price = 50, parking = 1, rating = 4, freeTicket = 0;
        string date = "2024-08-01", country = "USA";
        var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Title = "Event 1" },
            new Event { Id = Guid.NewGuid(), Title = "Event 2" }
        };
        _mockEventsRepository.Setup(repo => repo.GetAllFiltered(offset, limit, date, country, price, parking, rating, freeTicket)).Returns(events);

        // Act
        var result = _eventService.GetAllPaginated(offset, limit, date, country, price, parking, rating, freeTicket);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(events[0].Id, result[0].Id);
        Assert.Equal(events[1].Id, result[1].Id);
    }


    [Fact]
    public void ReserveSeatForUserOnEvent_ShouldThrowException_WhenEventHasEnded()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var pastDate = DateTime.Now.AddDays(-1).Date;

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = pastDate,
            ReservationCloseTime = new TimeOnly(23, 59),
            Capacity = 10
        };

        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId))
            .Returns(new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _eventService.reserveSeatForUserOnEvent(userId, eventId));

        Assert.Equal("Cannot reserve a seat. The event has already ended.", exception.Message);
    }

    [Fact]
    public void ReserveSeatForUserOnEvent_ShouldThrowException_WhenReservationTimeExpired()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var today = DateTime.Now.Date;
        var pastTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(-1));

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = today,
            ReservationCloseTime = pastTime,
            Capacity = 10
        };

        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId))
            .Returns(new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _eventService.reserveSeatForUserOnEvent(userId, eventId));

        Assert.Equal("Cannot reserve a seat. The event reservation has already ended.", exception.Message);
    }

    [Fact]
    public void ReserveSeatForUserOnEvent_ShouldThrowException_WhenEventIsFull()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var tomorrow = DateTime.Now.Date.AddDays(1);
        var reservationCloseTime = new TimeOnly(23, 59);

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = tomorrow,
            ReservationCloseTime = reservationCloseTime,
            Capacity = 2
        };

        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId))
            .Returns(new List<string> { "user1", "user2" });

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() =>
            _eventService.reserveSeatForUserOnEvent(userId, eventId));

        Assert.Equal("No seats available, the event is booked", exception.Message);
    }

    [Fact]
    public void ReserveSeatForUserOnEvent_ShouldThrowException_WhenUserAlreadyReserved()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var tomorrow = DateTime.Now.Date.AddDays(1);
        var anyTime = new TimeOnly(12, 0);

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = tomorrow,
            ReservationCloseTime = anyTime,
            Capacity = 10
        };

        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId))
            .Returns(new List<string> { userId });

        // Act & Assert
        var exception = Assert.Throws<AccessViolationException>(() =>
            _eventService.reserveSeatForUserOnEvent(userId, eventId));

        Assert.Equal("Can't Reserve more then 1 seat", exception.Message);
    }

    [Fact]
    public void ReserveSeatForUserOnEvent_ShouldSuccessfullyReserve_WhenConditionsAreMet()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var tomorrow = DateTime.Now.Date.AddDays(1);
        var anyTime = new TimeOnly(12, 0);

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = tomorrow,
            ReservationCloseTime = anyTime,
            Capacity = 10
        };

        var newEventUser = new EventUser { UserId = userId, EventId = eventId };

        _mockEventRepository.Setup(repo => repo.Get(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId))
            .Returns(new List<string>());
        _mockEventUserRepository.Setup(repo => repo.AddEventUser(userId, eventId))
            .Returns(newEventUser);

        // Act
        var result = _eventService.reserveSeatForUserOnEvent(userId, eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(eventId, result.EventId);
    }

    [Fact]
    public void CheckReservationForEvent_ShouldReturnTrue_WhenUserIsRegistered()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user1";
        var userList = new List<string> { userId };
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId)).Returns(userList);

        // Act
        var result = _eventService.checkReservationForEvent(eventId, userId);

        // Assert
        result.IsRegistered.Should().Be("true");
    }

    [Fact]
    public void CheckReservationForEvent_ShouldReturnFalse_WhenUserIsNotRegistered()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user1";
        var userList = new List<string> { "anotherUser" };
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId)).Returns(userList);

        // Act
        var result = _eventService.checkReservationForEvent(eventId, userId);

        // Assert
        result.IsRegistered.Should().Be("false");
    }

    [Fact]
    public void ParseEventFromExcel_ShouldReturnEvent_WhenFileIsValid()
    {
        // Arrange
        var file = CreateMockExcelFile();

        // Act
        var result = _eventService.ParseEventFromExcel(file);

        // Assert
        result.Should().NotBeNull();

        result.Title.Should().Be("Football Match");
        result.Country.Should().Be("USA");
        result.Address.Should().Be("123 Stadium Road");
        result.Rating.Should().Be(4.5f);
        result.Capacity.Should().Be(50000);
        result.Parking.Should().BeTrue();
        result.ImageUrl.Should().Be("https://example.com/image.jpg");
        result.Date.Should().Be(new DateTime(2025, 5, 10));
        result.StartTime.Should().Be(new TimeOnly(19, 0));
        result.EndTime.Should().Be(new TimeOnly(22, 0));
        result.GateOpenTime.Should().Be(new TimeOnly(17, 30));
        result.ReservationCloseTime.Should().Be(new TimeOnly(18, 30));
        result.Price.Should().Be(100);
        result.Label.Should().Be("Sports");
        result.Description.Should().Be("Exciting football match!");
    }

    [Fact]
    public void ParseEventFromExcel_ShouldThrowException_WhenFileIsNull()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _eventService.ParseEventFromExcel(null));

        Assert.Equal("No file uploaded.", exception.Message);
    }

    [Fact]
    public void ParseEventFromExcel_ShouldThrowException_WhenFileIsEmpty()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _eventService.ParseEventFromExcel(mockFile.Object));

        Assert.Equal("No file uploaded.", exception.Message);
    }

    [Fact]
    public void ParseEventFromExcel_ShouldReturnEvent_WhenValidExcelFileIsProvided()
    {
        // Arrange
        var file = CreateMockExcelFile();

        // Act
        var result = _eventService.ParseEventFromExcel(file);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Football Match", result.Title);
        Assert.Equal("USA", result.Country);
        Assert.Equal("123 Stadium Road", result.Address);
        Assert.Equal(4.5f, result.Rating);
        Assert.Equal(50000, result.Capacity);
        Assert.True(result.Parking);
        Assert.Equal("https://example.com/image.jpg", result.ImageUrl);
        Assert.Equal(new DateTime(2025, 5, 10), result.Date);
        Assert.Equal(new TimeOnly(19, 0), result.StartTime);
        Assert.Equal(new TimeOnly(22, 0), result.EndTime);
        Assert.Equal(new TimeOnly(17, 30), result.GateOpenTime);
        Assert.Equal(new TimeOnly(18, 30), result.ReservationCloseTime);
        Assert.Equal(100, result.Price);
        Assert.Equal("Sports", result.Label);
        Assert.Equal("Exciting football match!", result.Description);
    }

    private IFormFile CreateMockExcelFile()
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        // Headers
        string[] headers = { "Title", "Country", "Address", "Rating", "Capacity", "Parking", "ImageUrl",
                         "Date", "StartTime", "EndTime", "GateOpenTime", "ReservationCloseTime",
                         "Price", "Label", "Description" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        worksheet.Cell(2, 1).Value = "Football Match";
        worksheet.Cell(2, 2).Value = "USA";
        worksheet.Cell(2, 3).Value = "123 Stadium Road";
        worksheet.Cell(2, 4).Value = 4.5;
        worksheet.Cell(2, 5).Value = 50000;
        worksheet.Cell(2, 6).Value = true;
        worksheet.Cell(2, 7).Value = "https://example.com/image.jpg";
        worksheet.Cell(2, 8).Value = new DateTime(2025, 5, 10);
        worksheet.Cell(2, 9).Value = TimeSpan.Parse("19:00");
        worksheet.Cell(2, 10).Value = TimeSpan.Parse("22:00");
        worksheet.Cell(2, 11).Value = TimeSpan.Parse("17:30");
        worksheet.Cell(2, 12).Value = TimeSpan.Parse("18:30");
        worksheet.Cell(2, 13).Value = 100;
        worksheet.Cell(2, 14).Value = "Sports";
        worksheet.Cell(2, 15).Value = "Exciting football match!";

        var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(f => f.Length).Returns(memoryStream.Length);
        mockFile.Setup(f => f.FileName).Returns("test.xlsx");
        mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>()))
            .Callback<Stream>(s => memoryStream.CopyTo(s));

        return mockFile.Object;
    }

}
