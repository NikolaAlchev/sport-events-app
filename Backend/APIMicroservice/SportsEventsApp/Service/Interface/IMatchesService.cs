using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;
using Service.Implementation;

namespace Service.Interface
{
    public interface IMatchesService
    {
        public Task<List<MatchDTO>> getAllMatchesFromDate(DateOnly dateFrom);
        public Task<MatchDTO> getMatchById(int id);
        public Task<List<StandingsTableDTO>> getStandingsTableForComp(int homeTeamId, int awayTeamId);
        public Task<List<MatchDTO>> getHead2Head(int id);
    }
}
