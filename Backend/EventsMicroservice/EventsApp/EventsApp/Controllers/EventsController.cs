using System.Security.Claims;
using Domain.DTO;
using Domain.Model;
using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace EventsApp.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/Events/GetAllEvents
        [HttpGet("[action]")]
        public ActionResult<List<Event>> GetAllEvents()
        {

            var events = _eventService.GetAll();
            return Ok(events);
        }

        // GET: api/Events?offset=0&limit=6
        [HttpGet("")]
        public ActionResult<List<Event>> GetAllEventsPaginated([FromQuery] int offset = 0, [FromQuery] int limit = 6, [FromQuery] string date = "", [FromQuery] string country = "", [FromQuery] int price = 0, [FromQuery] int parking = 0,[FromQuery] int rating = 0, [FromQuery] int freeTicket = 0)
        {
            // *optional -> Add total count of events to the responce so that we can have page numbrers in the frontend
            if (limit > 6)
            {
                limit = 6;
            }
            var events = _eventService.GetAllPaginated(offset, limit, date, country, price, parking, rating, freeTicket);
            return Ok(events);
        }

        // GET: api/Events/SaveAsPDF?offset=0&limit=6
        [HttpGet("[action]")]
        public FileContentResult SaveAsPDF([FromQuery] int offset = 0, [FromQuery] int limit = 6, [FromQuery] string date = "", [FromQuery] string country = "", [FromQuery] int price = 0, [FromQuery] int parking = 0, [FromQuery] int rating = 0, [FromQuery] int freeTicket = 0)
        {
            if (limit > 6)
            {
                limit = 6;
            }
            var events = _eventService.GetAllPaginated(offset, limit, date, country, price, parking, rating, freeTicket);

            var template = DocumentModel.Load("Templates/Invoice.docx");

            var combinedDocument = new DocumentModel();

            //take the first event because GemBox free version only allows 20 paragraphs
            Event e = events.First();

            var document = template.Clone(true);

            document.Content.Replace("{Title}", e.Title);
            document.Content.Replace("{Country}", e.Country);
            document.Content.Replace("{Address}", e.Address);
            document.Content.Replace("{Rating}", e.Rating?.ToString());
            document.Content.Replace("{Capacity}", e.Capacity?.ToString());
            document.Content.Replace("{Parking}", e.Parking?.ToString());
            document.Content.Replace("{Date}", e.Date?.ToString("d"));
            document.Content.Replace("{StartTime}", e.StartTime?.ToString());
            document.Content.Replace("{EndTime}", e.EndTime?.ToString());
            document.Content.Replace("{GateOpenTime}", e.GateOpenTime?.ToString());
            document.Content.Replace("{ReservationCloseTime}", e.ReservationCloseTime?.ToString());
            document.Content.Replace("{Price}", e.Price?.ToString());
            document.Content.Replace("{Label}", e.Label);

            foreach (var section in document.Sections)
            {
                var importedSection = combinedDocument.Import(section, true);
                combinedDocument.Sections.Add(importedSection);
            }


            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "Events.pdf");
        }

        // GET: api/Events/GetEvent/{id}
        [HttpGet("[action]/{id}")]
        public ActionResult<Event> GetEvent(Guid id)
        {
            var evt = _eventService.GetEvent(id);
            if (evt == null)
            {
                return NotFound();
            }
            return Ok(evt);
        }

        // POST: api/Events/AddEvent
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Event> AddEvent([FromBody] Event e)
        {
            var createdEvent = _eventService.AddEvent(e);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        // PUT: api/Events/UpdateEvent/{id}
        [HttpPut("[action]")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Event> UpdateEvent([FromBody] Event e)
        {
            var updatedEvent = _eventService.UpdateEvent(e);
            if (updatedEvent == null)
            {
                return NotFound();
            }
            return Ok(updatedEvent);
        }

        // DELETE: api/Events/DeleteEvent/{id}
        [HttpDelete("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Event> DeleteEvent(Guid id)
        {
            var deletedEvent = _eventService.DeleteEvent(id);
            if (deletedEvent == null)
            {
                return NotFound();
            }
            return Ok(deletedEvent);
        }

        [HttpPost("register")]
        [Authorize (Roles = "User")]
        public IActionResult RegisterForEvent([FromBody] UserToEventDTO userEvent) 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User ID could not be found.");
            }


            try
            {
                _eventService.reserveSeatForUserOnEvent(userId, userEvent.EventId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return StatusCode(405, ex.Message);

            }
            catch (AccessViolationException ex)
            {
                return StatusCode(406, ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(407, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        [HttpGet("register/check")]
        [Authorize(Roles = "User")]
        public RegisteredDTO CheckRegistration(Guid eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User ID could not be found.");
            }

            return _eventService.checkReservationForEvent(eventId, userId);

        }


        [HttpPost("UploadExcel")]
        public IActionResult UploadExcel(IFormFile file)
        {
            try
            {
                var eventData = _eventService.ParseEventFromExcel(file);
                return Ok(eventData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


    }
}
