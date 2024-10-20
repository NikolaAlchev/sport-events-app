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
    }
}
