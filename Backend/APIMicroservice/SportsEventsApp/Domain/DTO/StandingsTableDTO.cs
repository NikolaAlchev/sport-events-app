using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class StandingsTableDTO
    {
        public int CompetitionId { get; set; }

        public string CompetitionName {  get; set; }
        
        public List<TeamPositionDTO> teamStandings { get; set; }


    }
}
