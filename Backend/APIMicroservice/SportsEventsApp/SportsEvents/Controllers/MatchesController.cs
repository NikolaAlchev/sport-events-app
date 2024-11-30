using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchesService _matchesService;


        public MatchesController(IMatchesService matchesService) { 
            _matchesService = matchesService;
        }

        [HttpGet("all")]
        public async Task<List<MatchDTO>> getAllMatchesOnDate(DateOnly fromDate)
        {
            return await _matchesService.getAllMatchesFromDate(fromDate);
        }

        [HttpGet("{id}")]
        public async Task<MatchDTO> getMatchById(int id)
        {
            return await _matchesService.getMatchById(id);
        }

        [HttpGet("standings")]
        public async Task<List<StandingsTableDTO>> getStandings([FromQuery] int homeTeam, [FromQuery] int awayTeam)
        {
            return await _matchesService.getStandingsTableForComp(homeTeam, awayTeam);
        }

        [HttpGet("head2head")]
        public async Task<List<MatchDTO>> getHead2Head([FromQuery] int id)
        {
            return await _matchesService.getHead2Head(id);
        }

    }
}
