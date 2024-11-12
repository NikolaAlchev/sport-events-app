using Domain.DTO;
using Domain.Model;

namespace Service.Interface
{
    public interface IEventService
    {
        List<Event> GetAll();
        Event GetEvent(Guid? id);
        Event AddEvent(Event e);
        Event UpdateEvent(Event e);
        Event DeleteEvent(Guid id);
        List<Event> GetAllPaginated(int offset , int limit , string date, string country,  int price ,  int parking , int rating);
        public EventUser reserveSeatForUserOnEvent(string UserId, Guid EventId);
        public RegisteredDTO checkReservationForEvent(Guid eventId, string userId);
    }
}
