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
        private ICompetitionsRepo _competitionsRepo;
        public MatchesService(IMatchesRepo matchesRepo, ICompetitionsRepo competitionsRepo) { 
            _matchesRepo = matchesRepo;
            _competitionsRepo = competitionsRepo;
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

        public async Task<List<StandingsTableDTO>> getStandingsTableForComp(int homeTeamId, int awayTeamId)
        {
            int homeTeamLeagueId = await _competitionsRepo.standingsForLeagueFromTeamId(homeTeamId);
            if (homeTeamLeagueId == 0)
            {
                throw new Exception("no league found for the home team");
            }
            int awayTeamLeagueId = await _competitionsRepo.standingsForLeagueFromTeamId(awayTeamId);
            if (awayTeamLeagueId == 0)
            {
                throw new Exception("no league found for the away team");
            }

            var res = new List<StandingsTableDTO>();
            if (homeTeamLeagueId != awayTeamLeagueId)
            {

                res.Add(await _matchesRepo.getStandingsTable(homeTeamLeagueId));
                res.Add(await _matchesRepo.getStandingsTable(awayTeamId));
                return res;
            }

            res.Add(await _matchesRepo.getStandingsTable(homeTeamLeagueId));
            return res;
        }


        public async Task<List<MatchDTO>> getHead2Head(int id)
        {
            return await _matchesRepo.getHead2Head(id);
        }


    }
}
