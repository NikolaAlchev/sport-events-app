using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Repository.Interface
{
    public interface IMatchesRepo
    {
        public Task<List<MatchDTO>> getAllMatchesFromDate(DateOnly dateFrom);
        public Task<MatchDTO> getMatchById(int id);
        public Task<StandingsTableDTO> getStandingsTable(int competitionId);
        public Task<List<MatchDTO>> getHead2Head(int id);

    }
}
