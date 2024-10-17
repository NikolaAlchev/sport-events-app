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
    public class PlayerService: IPlayerService
    {
        private IPlayerRepo _playerRepo;

        public PlayerService(IPlayerRepo playerRepo)
        {
            _playerRepo = playerRepo;
        }

        public async Task<PlayerDTO> GetPlayerInfo(int id)
        {
            return await _playerRepo.GetPlayer(id);
        }

        public async Task<List<TopScorerDTO>> TopScorersFromCompId(int id)
        {
            return await _playerRepo.TopScorersFromCompId(id);
        }
    }
}
