using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Service.Interface
{
    public interface IPlayerService
    {
        public Task<PlayerDTO> GetPlayerInfo(int id);
    }
}
