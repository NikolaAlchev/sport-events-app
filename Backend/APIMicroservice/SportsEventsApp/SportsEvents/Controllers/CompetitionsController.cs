using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompetitionsController : ControllerBase
    {
        private readonly ICompetitionsService _compService;


        public CompetitionsController(ICompetitionsService compService)
        {
            _compService = compService;
        }

        [HttpGet("all")]
        public async Task<List<CompetitionDTO>> getAllCompetitions()
        {
            return await _compService.getAllCompetitions();
        }

        [HttpGet("{id}/teams")]
        public async Task<List<TeamDTO>> getTeamsByCompId(int id)
        {
            return await _compService.getTeamsByCompId(id);
        }


/*        [HttpGet("team/standings/{id}")]
        public async Task<StandingsTableDTO> getStandings(int id)
        {
            return await _compService.getStandingsTableForComp(id);
        }*/

    }
}
