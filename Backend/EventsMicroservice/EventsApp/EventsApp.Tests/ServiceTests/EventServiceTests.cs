using Moq;
using Xunit;
using Service.Implementation;
using Domain.Model;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Domain.DTO;
using System.IO;
using ClosedXML.Excel;

public class EventServiceTests
{
    private readonly Mock<IRepository<Event>> _mockEventRepository;
    private readonly Mock<IEventsRepository> _mockEventsRepository;
    private readonly Mock<IEventUserRepository> _mockEventUserRepository;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
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
    public void ReserveSeatForUserOnEvent_ShouldThrowException_WhenEventHasEnded()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "user123";
        var pastDate = DateTime.Now.AddDays(-1);

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = pastDate,
            ReservationCloseTime = new TimeOnly(23, 59),
            Capacity = 10,
            Users = new List<EventUser>()
        };

        _mockEventsRepository.Setup(repo => repo.GetEventWithUsers(eventId)).Returns(sportsEvent);

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
        var pastTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(-1)); // Reservation closed an hour ago

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = today,
            ReservationCloseTime = pastTime,
            Capacity = 10,
            Users = new List<EventUser>()
        };

        _mockEventsRepository.Setup(repo => repo.GetEventWithUsers(eventId)).Returns(sportsEvent);

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
        var today = DateTime.Now.Date;
        var futureTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2)); // Still open for reservations

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = today,
            ReservationCloseTime = futureTime,
            Capacity = 2, // Event capacity is full
            Users = new List<EventUser>
                {
                    new EventUser { UserId = "user1", EventId = eventId },
                    new EventUser { UserId = "user2", EventId = eventId }
                }
        };

        _mockEventsRepository.Setup(repo => repo.GetEventWithUsers(eventId)).Returns(sportsEvent);

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
        var today = DateTime.Now.Date;
        var futureTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = today,
            ReservationCloseTime = futureTime,
            Capacity = 10,
            Users = new List<EventUser>
                {
                    new EventUser { UserId = userId, EventId = eventId } // User already reserved
                }
        };

        _mockEventsRepository.Setup(repo => repo.GetEventWithUsers(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId)).Returns(new List<string> { userId });

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
        var today = DateTime.Now.Date;
        var futureTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));

        var sportsEvent = new Event
        {
            Id = eventId,
            Date = today,
            ReservationCloseTime = futureTime,
            Capacity = 10,
            Users = new List<EventUser>()
        };

        var newEventUser = new EventUser { UserId = userId, EventId = eventId };

        _mockEventsRepository.Setup(repo => repo.GetEventWithUsers(eventId)).Returns(sportsEvent);
        _mockEventUserRepository.Setup(repo => repo.getUsersFromEvent(eventId)).Returns(new List<string>());
        _mockEventUserRepository.Setup(repo => repo.AddEventUser(userId, eventId)).Returns(newEventUser);

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

    /*    [Fact]
        public void ParseEventFromExcel_ShouldReturnEvent_WhenFileIsValid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var filePath = "testfile.xlsx";
            var stream = new MemoryStream(File.ReadAllBytes(filePath));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(filePath);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            // Act
            var result = _eventService.ParseEventFromExcel(fileMock.Object);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Event");
        }*/

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

        // Creating Header Row
        string[] headers = { "Title", "Country", "Address", "Rating", "Capacity", "Parking", "ImageUrl",
                         "Date", "StartTime", "EndTime", "GateOpenTime", "ReservationCloseTime",
                         "Price", "Label", "Description" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Creating Data Row
        string[] data = { "Football Match", "USA", "123 Stadium Road", "4.5", "50000", "true",
                      "https://example.com/image.jpg", "2025-05-10", "19:00", "22:00", "17:30",
                      "18:30", "100", "Sports", "Exciting football match!" };

        for (int i = 0; i < data.Length; i++)
        {
            worksheet.Cell(2, i + 1).Value = data[i];
        }

        var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(memoryStream.ToArray())); // NEW: Return a fresh stream
        mockFile.Setup(f => f.Length).Returns(memoryStream.Length);
        mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s =>
        {
            var freshStream = new MemoryStream(memoryStream.ToArray());
            freshStream.CopyTo(s);
        });

        return mockFile.Object;
    }

}
