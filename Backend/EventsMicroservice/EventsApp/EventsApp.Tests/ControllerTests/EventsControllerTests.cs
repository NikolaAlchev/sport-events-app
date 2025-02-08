using Domain.DTO;
using Domain.Model;
using EventsApp.Controllers;
using GemBox.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interface;
using System.Security.Claims;

public class EventsControllerTests
{
    private readonly Mock<IEventService> _mockEventService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        _mockEventService = new Mock<IEventService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _controller = new EventsController(_mockEventService.Object);
    }

    [Fact]
    public void GetAllEvents_ReturnsOkResultWithEvents()
    {
        // Arrange
        var mockEvents = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Title = "Event 1" },
            new Event { Id = Guid.NewGuid(), Title = "Event 2" }
        };
        _mockEventService.Setup(s => s.GetAll()).Returns(mockEvents);

        // Act
        var result = _controller.GetAllEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<List<Event>>(okResult.Value);
        Assert.Equal(2, events.Count);
    }

    [Fact]
    public void GetAllEvents_ReturnsEmptyListWhenNoEvents()
    {
        // Arrange
        _mockEventService.Setup(s => s.GetAll()).Returns(new List<Event>());

        // Act
        var result = _controller.GetAllEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<List<Event>>(okResult.Value);
        Assert.Empty(events);
    }



    [Fact]
    public void GetAllEventsPaginated_ValidParameters_ReturnsOk()
    {
        // Arrange
        var events = new List<Event>
    {
        new Event { Id = Guid.NewGuid(), Title = "Event 1", Date = DateTime.Now },
        new Event { Id = Guid.NewGuid(), Title = "Event 2", Date = DateTime.Now }
    };

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        // Act
        var result = _controller.GetAllEventsPaginated(0, 6, "", "", 0, 0, 0, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Equal(events.Count, returnValue.Count);
    }

    [Fact]
    public void GetAllEventsPaginated_LimitGreaterThanSix_SetsLimitToSix()
    {
        // Arrange
        var events = new List<Event> { new Event { Id = Guid.NewGuid(), Title = "Event 1", Date = DateTime.Now } };

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), 6, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        // Act
        var result = _controller.GetAllEventsPaginated(0, 10, "", "", 0, 0, 0, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Equal(events.Count, returnValue.Count);
    }

    [Fact]
    public void GetAllEventsPaginated_EmptyResponse_ReturnsEmptyList()
    {
        // Arrange
        var events = new List<Event>();

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        // Act
        var result = _controller.GetAllEventsPaginated(0, 6, "", "", 0, 0, 0, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public void GetAllEventsPaginated_InvalidLimit_ReturnsOkWithEmptyList()
    {
        // Arrange
        var events = new List<Event>();

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        // Act
        var result = _controller.GetAllEventsPaginated(0, -1, "", "", 0, 0, 0, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public void SaveAsPDF_ValidParameters_ReturnsPdfFile()
    {
        // Arrange
        var events = new List<Event>
    {
        new Event {
            Id = Guid.NewGuid(),
            Title = "Event 1",
            Country = "USA",
            Address = "123 Main St",
            Rating = 4.5f,
            Capacity = 200,
            Parking = true,
            Date = DateTime.Now,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(12, 0),
            GateOpenTime = new TimeOnly(9, 30),
            ReservationCloseTime = new TimeOnly(9, 0),
            Price = 50,
            Label = "VIP"
        }
    };

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        var baseDirectory = AppContext.BaseDirectory;
        var templatePath = Path.Combine(baseDirectory, "Templates", "Invoice.docx");

        Assert.True(File.Exists(templatePath), $"Template not found at {templatePath}");

        // Act
        var result = _controller.SaveAsPDF(0, 6, "", "", 0, 0, 0, 0);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal("Events.pdf", fileResult.FileDownloadName);
        Assert.NotEmpty(fileResult.FileContents);
    }


    [Fact]
    public void SaveAsPDF_LimitGreaterThanSix_SetsLimitToSix()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event {
                Id = Guid.NewGuid(),
                Title = "Event 1",
                Country = "USA",
                Address = "123 Main St",
                Rating = 4.5f,
                Capacity = 200,
                Parking = true,
                Date = DateTime.Now,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                GateOpenTime = new TimeOnly(9, 30),
                ReservationCloseTime = new TimeOnly(9, 0),
                Price = 50,
                Label = "VIP"
            }
        };

        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), 6, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(events);

        var baseDirectory = AppContext.BaseDirectory;
        var templatePath = Path.Combine(baseDirectory, "Templates", "Invoice.docx");

        Assert.True(File.Exists(templatePath), $"Template not found at {templatePath}");


        // Act
        var result = _controller.SaveAsPDF(0, 10, "", "", 0, 0, 0, 0);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal("Events.pdf", fileResult.FileDownloadName);
        Assert.NotEmpty(fileResult.FileContents);
    }

    [Fact]
    public void SaveAsPDF_NoEvents_ThrowsException()
    {
        // Arrange
        _mockEventService.Setup(service => service.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new List<Event>());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _controller.SaveAsPDF(0, 6, "", "", 0, 0, 0, 0));
        Assert.Equal("No events found for the given filters.", exception.Message);
    }



    [Fact]
    public void GetEvent_ValidId_ReturnsOkResultWithEvent()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventObj = new Event
        {
            Id = eventId,
            Title = "Event",
            Country = "USA",
            Address = "123 Main St",
            Rating = 4.5f,
            Capacity = 100,
            Price = 20
        };

        _mockEventService.Setup(service => service.GetEvent(eventId))
            .Returns(eventObj);

        // Act
        var result = _controller.GetEvent(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Event>(okResult.Value);
        Assert.Equal(eventId, returnValue.Id);
    }

    [Fact]
    public void GetEvent_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockEventService.Setup(service => service.GetEvent(eventId))
            .Returns((Event)null);

        // Act
        var result = _controller.GetEvent(eventId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }


    [Fact]
    public void AddEvent_ShouldReturnCreatedAtAction_WhenEventIsCreated()
    {
        // Arrange
        var newEvent = new Event { Title = "New Event" };
        _mockEventService.Setup(service => service.AddEvent(newEvent)).Returns(newEvent);

        // Act
        var result = _controller.AddEvent(newEvent);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetEvent", createdAtActionResult.ActionName);
    }


    [Fact]
    public void UpdateEvent_ValidEvent_ReturnsOk()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventToUpdate = new Event
        {
            Id = eventId,
            Title = "Updated Event",
            Country = "USA",
            Date = DateTime.Now
        };

        _mockEventService.Setup(service => service.UpdateEvent(It.IsAny<Event>()))
            .Returns(eventToUpdate);

        // Act
        var result = _controller.UpdateEvent(eventToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Event>(okResult.Value);
        Assert.Equal(eventId, returnValue.Id);
    }

    [Fact]
    public void UpdateEvent_EventNotFound_ReturnsNotFound()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventToUpdate = new Event
        {
            Id = eventId,
            Title = "Event",
            Country = "USA",
            Date = DateTime.Now
        };

        _mockEventService.Setup(service => service.UpdateEvent(It.IsAny<Event>()))
            .Returns((Event)null);

        // Act
        var result = _controller.UpdateEvent(eventToUpdate);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void DeleteEvent_ValidId_ReturnsOk()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventToDelete = new Event
        {
            Id = eventId,
            Title = "Event",
            Country = "USA",
            Date = DateTime.Now
        };

        _mockEventService.Setup(service => service.DeleteEvent(It.IsAny<Guid>()))
            .Returns(eventToDelete);

        // Act
        var result = _controller.DeleteEvent(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Event>(okResult.Value);
        Assert.Equal(eventId, returnValue.Id);
    }

    [Fact]
    public void DeleteEvent_EventNotFound_ReturnsNotFound()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockEventService.Setup(service => service.DeleteEvent(It.IsAny<Guid>()))
            .Returns((Event)null);

        // Act
        var result = _controller.DeleteEvent(eventId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void RegisterForEvent_ValidUser_ReturnsOk()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };
        var userId = "123";

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.reserveSeatForUserOnEvent(userId, userEvent.EventId))
            .Verifiable();

        // Act
        var result = _controller.RegisterForEvent(userEvent);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        _mockEventService.Verify();
    }



    [Fact]
    public void RegisterForEvent_UserIdNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };

        var claimsIdentity = new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _controller.RegisterForEvent(userEvent));
        Assert.Equal("User ID could not be found.", exception.Message);
    }



    [Fact]
    public void RegisterForEvent_ArgumentException_ReturnsStatus405()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };
        var userId = "123";

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.reserveSeatForUserOnEvent(userId, userEvent.EventId))
            .Throws(new ArgumentException("Invalid argument"));

        // Act
        var result = _controller.RegisterForEvent(userEvent);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(405, statusCodeResult.StatusCode);
        Assert.Equal("Invalid argument", statusCodeResult.Value);
    }



    [Fact]
    public void RegisterForEvent_AccessViolationException_ReturnsStatus406()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };
        var userId = "123";

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.reserveSeatForUserOnEvent(userId, userEvent.EventId))
            .Throws(new AccessViolationException("Access violation"));

        // Act
        var result = _controller.RegisterForEvent(userEvent);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(406, statusCodeResult.StatusCode);
        Assert.Equal("Access violation", statusCodeResult.Value);
    }



    [Fact]
    public void RegisterForEvent_ApplicationException_ReturnsStatus407()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };
        var userId = "123";

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.reserveSeatForUserOnEvent(userId, userEvent.EventId))
            .Throws(new ApplicationException("Application error"));

        // Act
        var result = _controller.RegisterForEvent(userEvent);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(407, statusCodeResult.StatusCode);
        Assert.Equal("Application error", statusCodeResult.Value);
    }



    [Fact]
    public void RegisterForEvent_GeneralException_ReturnsStatus500()
    {
        // Arrange
        var userEvent = new UserToEventDTO { EventId = Guid.NewGuid() };
        var userId = "123";

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.reserveSeatForUserOnEvent(userId, userEvent.EventId))
            .Throws(new Exception("General error"));

        // Act
        var result = _controller.RegisterForEvent(userEvent);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("General error", statusCodeResult.Value);
    }


    [Fact]
    public void CheckRegistration_ValidUser_ReturnsRegisteredDTO()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = "123";
        var expectedResult = new RegisteredDTO { IsRegistered = "true" };

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEventService.Setup(service => service.checkReservationForEvent(eventId, userId)).Returns(expectedResult);

        // Act
        var result = _controller.CheckRegistration(eventId);

        // Assert
        var registeredDto = Assert.IsType<RegisteredDTO>(result);
        Assert.Equal("true", registeredDto.IsRegistered);
    }



    [Fact]
    public void CheckRegistration_UserIdNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _controller.CheckRegistration(eventId));
        Assert.Equal("User ID could not be found.", exception.Message);
    }


    [Fact]
    public void UploadExcel_ValidFile_ReturnsOk()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3 };
        var fileName = "test.xlsx";

        var memoryStream = new MemoryStream(fileContent);
        mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.Length).Returns(fileContent.Length);

        _mockEventService.Setup(service => service.ParseEventFromExcel(mockFile.Object))
            .Returns(new Event { Id = Guid.NewGuid(), Title = "Sample Event" });

        // Act
        var result = _controller.UploadExcel(mockFile.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }



    [Fact]
    public void UploadExcel_ArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3 };
        var fileName = "test.xlsx";

        var memoryStream = new MemoryStream(fileContent);
        mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.Length).Returns(fileContent.Length);

        _mockEventService.Setup(service => service.ParseEventFromExcel(mockFile.Object))
            .Throws(new ArgumentException("Invalid file format"));

        // Act
        var result = _controller.UploadExcel(mockFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Invalid file format", badRequestResult.Value);
    }

    [Fact]
    public void UploadExcel_GeneralException_ReturnsInternalServerError()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3 };
        var fileName = "test.xlsx";

        var memoryStream = new MemoryStream(fileContent);
        mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.Length).Returns(fileContent.Length);

        _mockEventService.Setup(service => service.ParseEventFromExcel(mockFile.Object))
            .Throws(new Exception("Unexpected error"));

        // Act
        var result = _controller.UploadExcel(mockFile.Object);

        // Assert
        var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        Assert.Equal("Internal server error: Unexpected error", internalServerErrorResult.Value);
    }
}
