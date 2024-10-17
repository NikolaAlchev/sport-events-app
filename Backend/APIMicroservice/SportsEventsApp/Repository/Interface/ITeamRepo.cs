using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Repository.Interface
{
    public interface ITeamRepo
    {
        public Task<TeamPositionDTO> GetTeamByCompetitionIdAndName(int id,string name);

        public Task<List<BasicPlayerInfoDTO>> GetSquadFromTeamId(int id);
    }
}
