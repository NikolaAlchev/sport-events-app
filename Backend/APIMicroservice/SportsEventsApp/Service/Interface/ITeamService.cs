using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Service.Interface
{
    public interface ITeamService
    {
        public Task<TeamPositionDTO> GetTeamByCompetitionIdAndName(int id, string teamName);
        public Task<List<BasicPlayerInfoDTO>>GetSquadFromTeamId(int id);
    }
}
