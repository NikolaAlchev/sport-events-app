using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class MatchDTO
    {
        public int Id { get; set; }

        public DateTime UtcDate { get; set; }

        public string Status { get; set; }

        public int HomeTeamId {  get; set; }
        
        public string HomeTeamName { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get;set; }

        public int HomeTeamScore { get; set; }

        public int AwayTeamScore { get;set; }

        public string CompetitionName { get; set; }
        public int CompetitionId { get; set;}

        public string HomeTeamCrest { get; set; }

        public string AwayTeamCrest { get;set; }

        public string Venue {  get; set; }

    }
}
