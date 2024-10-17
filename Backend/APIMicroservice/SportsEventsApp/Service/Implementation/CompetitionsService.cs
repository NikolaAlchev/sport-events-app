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
    public class CompetitionsService: ICompetitionsService
    {
        private ICompetitionsRepo _competitionsRepo;
        public CompetitionsService(ICompetitionsRepo competitionsRepo) {
            _competitionsRepo = competitionsRepo;
                
         }

        public async Task<List<CompetitionDTO>> getAllCompetitions()
        {
            return await _competitionsRepo.getAllCompetitions();
        }

        public async Task<List<TeamDTO>> getTeamsByCompId(int id)
        {
            return await _competitionsRepo.getAllTeamsFromCompetition(id);
        }
    }
}
