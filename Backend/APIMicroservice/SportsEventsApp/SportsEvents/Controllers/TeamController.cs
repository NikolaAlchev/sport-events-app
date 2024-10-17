using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("{id}")]
        public async Task<TeamPositionDTO> GetTeamByCompIdAndTeamName(int id, [FromQuery] string name)
        {

            return await _teamService.GetTeamByCompetitionIdAndName(id, name);

        }

        [HttpGet("{id}/roster")]
        public async Task<List<BasicPlayerInfoDTO>> GetSquadOfTeamByTeamId(int id)
        {
            return await _teamService.GetSquadFromTeamId( id);
        }




    }
}
