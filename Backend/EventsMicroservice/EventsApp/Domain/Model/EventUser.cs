using Domain.Identity;

namespace Domain.Model
{
    public class EventUser : BaseEntity
    {
        public Guid? EventId { get; set; }
        public Event? Event { get; set; }

        public string? UserId { get; set; }
        public EventsAppUser? User { get; set; }
    }
}
