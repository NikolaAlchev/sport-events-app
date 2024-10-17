using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class CompetitionDTO
    {
        public int Id { get; set; }
        public string Country { get; set; }

        public string CountryFlag{ get; set; }
        public string Name { get; set; }

        public string Emblem { get; set; }
        
        public string Type { get; set; }
        public int CurrentSeasonId { get; set; }

        public DateOnly CurrentSeasonStartDate { get; set; }

        public DateOnly CurrentSeasonEndDate { get; set; }

        public int CurrentMatchDay { get; set; }




    }
}
