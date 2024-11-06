using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Repository.Interface
{
    public interface IEventsRepository
    {
        public List<Event> GetAllFiltered(int offset, int limit, string date, string country, int price, int parking, int rating);
        public List<EventUser> getEventUsers(Guid eventId);
        public Event GetEventWithUsers(Guid eventId);
    }
}
