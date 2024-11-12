using System.ComponentModel;
using Domain.DTO;
using Domain.Identity;
using Domain.Model;
using Repository.Implementation;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IEventUserRepository _eventUserRepository;
        public EventService(IRepository<Event> eventRepository, IEventsRepository eventsRepository , IEventUserRepository eventUserRepository)
        {
            _eventRepository = eventRepository;
            _eventsRepository = eventsRepository;
            _eventUserRepository = eventUserRepository;  
        }

        public Event AddEvent(Event e)
        {
            return _eventRepository.Insert(e);
        }

        public Event DeleteEvent(Guid id)
        {
            return _eventRepository.Delete(GetEvent(id));
        }

        public List<Event> GetAll()
        {
            return _eventRepository.GetAll().ToList();
        }

        public Event GetEvent(Guid? id)
        {
            return _eventRepository.Get(id);
        }

        public Event UpdateEvent(Event e)
        {
            return _eventRepository.Update(e);
        }

        public List<Event> GetAllPaginated(int offset, int limit)
        {
            return _eventRepository.GetAllPaginated(offset,limit).ToList();
        }

        public List<Event> GetAllPaginated(int offset, int limit, string date, string country, int price, int parking, int rating)
        {
            return _eventsRepository.GetAllFiltered(offset, limit, date, country, price,parking, rating);
        }

        public EventUser reserveSeatForUserOnEvent(string UserId, Guid EventId)
        {
            var sportsEvent = _eventsRepository.GetEventWithUsers(EventId);
            var userIds = _eventUserRepository.getUsersFromEvent(EventId);

            DateTime resDate = sportsEvent.Date.Value.Date.AddDays(-1); // Adjust the event date to the day before
            TimeOnly timeClosedRes = sportsEvent.ReservationCloseTime.Value;
            DateTime now = DateTime.Now;
            DateTime currentDate = now.Date;

            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

            if (resDate < currentDate)
            {
             
                throw new ArgumentException("Cannot reserve a seat. The event has already ended.");
             
            }
            else if (resDate == currentDate)
            {
                if (currentTime > timeClosedRes)
                {
                    throw new ArgumentException("Cannot reserve a seat. The event reservation has already ended.");
                }
            }
            List<EventUser> eventUsers = sportsEvent.Users.ToList();
            if (eventUsers.Count() == sportsEvent.Capacity.Value)
            {
                throw new ApplicationException("No seats available, the event is booked");

            }
            if (!userIds.Contains(UserId))
            {
                    var eventUser = _eventUserRepository.AddEventUser(UserId, EventId);
                    return eventUser;

            }
            throw new AccessViolationException("Can't Reserve more then 1 seat");




        }

        public RegisteredDTO checkReservationForEvent(Guid eventId, string userId)
        {
            List<string> usersFromEvent = _eventUserRepository.getUsersFromEvent(eventId);
            if (usersFromEvent.Contains(userId))
            {

                return new RegisteredDTO { IsRegistered = "true"};
            }
            return new RegisteredDTO { IsRegistered = "false" };
        }
    }
}
