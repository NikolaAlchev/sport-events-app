using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class TopScorerDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nationality { get; set; }

        public string Position { get; set; }

        public int ShirtNumber { get; set; }

        public int CurrentTeamId { get; set; }

        public string CurrentTeamName { get; set; }

        public string CurrentTeamCrest { get; set; }

        public int PlayedMatches { get; set; }

        public int Goals { get; set; }

        public int Assists { get; set; }

        public int Penalties { get; set; }

    }
}
