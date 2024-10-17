using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Service.Interface
{
    public interface ICompetitionsService
    {

        public Task<List<CompetitionDTO>> getAllCompetitions();
        public Task<List<TeamDTO>> getTeamsByCompId(int id);
    }
}
