using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class TeamInfoDTO
    {
        public string Name { get; set; }
        public string Venue { get; set; }
        public string Crest { get; set; }
        public List<MatchDTO> PastMatches { get; set; }
        public List<MatchDTO> FutureMatches { get; set; }
        public List<BasicPlayerInfoDTO> Squad { get; set; }
    }
}
