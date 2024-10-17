using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Repository.Interface
{
    public interface IPlayerRepo
    {
        public Task<PlayerDTO> GetPlayer(int id);
    }
}
