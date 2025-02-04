﻿using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{id}")]
        public async Task<PlayerDTO> GetPlayerInfo(int id)
        {
            return await _playerService.GetPlayerInfo(id);
        }


        [HttpGet("{competitionId}/topScorers")]
        public async Task<List<TopScorerDTO>> GetTopScorersFromComp(int competitionId)
        {
            return await _playerService.TopScorersFromCompId(competitionId);
        }
    }
}
