using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class EventUserRepository : IEventUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EventUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public EventUser getEventUser(string userId, Guid eventId)
        {
            return _context.EventUsers.Where(i => i.EventId.Equals(eventId) && i.UserId.Equals(userId)).First();
        }

        public EventUser AddEventUser(string userId, Guid eventId)
        {
            var user  = _context.Users.Where(i => i.Id.Equals(userId)).First();
            var eventFromEventId = _context.Events.Where(i => i.Id.Equals(eventId)).First();

            var newEventUser = new EventUser
            {
                UserId = userId,
                EventId = eventId,
                Event = eventFromEventId,
                User = user
            };
            _context.EventUsers.Add(newEventUser);

            eventFromEventId.Users.Add(newEventUser);


            _context.SaveChanges();

            return newEventUser;
        }
    }
}
