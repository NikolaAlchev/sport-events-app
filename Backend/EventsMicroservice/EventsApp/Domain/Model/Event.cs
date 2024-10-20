using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Event : BaseEntity
    {
        public string? Title { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public float? Rating { get; set; }
        public int? Capacity { get; set; }
        public bool? Parking { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeOnly? GateOpenTime { get; set; }
        public TimeOnly? ReservationCloseTime { get; set; }
        public int? Price { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<EventUser>? Users { get; set; }

    }
}
