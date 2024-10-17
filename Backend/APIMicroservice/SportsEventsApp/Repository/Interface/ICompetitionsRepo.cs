using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Repository.Interface
{
    public interface ICompetitionsRepo
    {

        public Task<List<CompetitionDTO>> getAllCompetitions();
        public Task<List<TeamDTO>>getAllTeamsFromCompetition(int id);
    }
}
