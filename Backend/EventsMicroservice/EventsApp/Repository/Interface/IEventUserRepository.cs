using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Repository.Interface
{
    public interface IEventUserRepository
    {
        public EventUser getEventUser(string userId, Guid eventId);
        public EventUser AddEventUser(string userId, Guid eventId);
    }
}
