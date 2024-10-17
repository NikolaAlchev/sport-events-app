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
    public class TeamService : ITeamService
    {
        private ITeamRepo _teamRepo;
        public TeamService(ITeamRepo teamRepo) 
        {
            _teamRepo = teamRepo;
        }

        public async Task<List<BasicPlayerInfoDTO>> GetSquadFromTeamId(int id)
        {
            return await _teamRepo.GetSquadFromTeamId(id);
        }

        public async Task<TeamPositionDTO> GetTeamByCompetitionIdAndName(int id, string teamName)
        {
            return await _teamRepo.GetTeamByCompetitionIdAndName(id, teamName);
        }
    }
}
