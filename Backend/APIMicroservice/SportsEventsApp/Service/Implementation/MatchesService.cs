using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class MatchesService : IMatchesService
    {
        private IMatchesRepo _matchesRepo;
        public MatchesService(IMatchesRepo matchesRepo) { 
            _matchesRepo = matchesRepo;
        }
        public async Task<List<MatchDTO>> getAllMatchesFromDate(DateOnly dateFrom)
        {
            return await _matchesRepo.getAllMatchesFromDate(dateFrom);
        }

        public async Task<MatchDTO> getMatchById(int id)
        {
            return await _matchesRepo.getMatchById(id);
        }

        public async Task<StandingsTableDTO> getStandingsTableForComp( int compId)
        {
            return await _matchesRepo.getStandingsTable(compId);
        }
    }
}
