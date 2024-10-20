using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
