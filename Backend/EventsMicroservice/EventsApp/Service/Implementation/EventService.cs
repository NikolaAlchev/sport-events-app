using Domain.Model;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;

        public EventService(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
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
    }
}
