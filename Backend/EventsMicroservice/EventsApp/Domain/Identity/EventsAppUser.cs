using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class EventsAppUser : IdentityUser
    {
        public ICollection<EventUser>? Events { get; set; }
    }
}
