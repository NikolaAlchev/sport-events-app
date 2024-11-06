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
    public class EventRepository:IEventsRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public Event GetEventWithUsers(Guid eventId) {

            return _context.Events
                .Where(i => i.Id.Equals(eventId)).FirstOrDefault();
        
        }   


        public List<Event> GetAllFiltered(int offset, int limit, string date, string country, int price, int parking, int rating)
        {
            //fix price filter 
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
               
                DateTime eventDate;
                if (DateTime.TryParse(date, out eventDate))
                {
                    query = query.Where(e => e.Date == eventDate);
                }
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(e => e.Country.ToLower().Contains(country.ToLower()) );
            }

            if (price > 0)
            {
                if (price == 1)
                {
                    query = query.OrderBy(e => e.Price);
                }
                else
                {
                    query = query.OrderByDescending(e => e.Price);
                }
            }

            if (parking > 0)
            {
                if (parking == 1)
                {
                    query = query.Where(e => e.Parking == true);
                }
                else
                {
                    query = query.Where(e => e.Parking == false);
                }
               
            }

            if (rating > 0)
            {
                query = query.Where(e => e.Rating >= rating);
            }

            return query.Skip(offset).Take(limit).ToList();
        }

        public List<EventUser> getEventUsers(Guid eventId)
        {
            try
            {
                
                var eventEntity = _context.Events
                    .Where(i => i.Id.Equals(eventId))
                    .FirstOrDefault()?.Users?.ToList();

               if (eventEntity == null)
                {
                    throw new Exception($"Event with ID {eventId} not found.");
                }


                if (eventEntity.Count.Equals(0))
                {
                    return new List<EventUser> ();
                }
                return eventEntity;
            }
            catch (Exception ex)
            {
                // Handle any other exceptions (e.g., database issues)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }


    }
}
