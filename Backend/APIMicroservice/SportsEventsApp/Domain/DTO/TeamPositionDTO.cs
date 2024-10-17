using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class TeamPositionDTO
    {
        public string TeamName { get; set; }

        public int TeamId {  get; set; }

        public string TeamCrest {  get; set; }

        public int Position {  get; set; }

        public int PlayedGames { get; set; }


        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Draws { get; set; }

        public int Points { get; set; }

        public int GoalsFor {  get; set; }

        public int GoalsAgainst { get; set; }


    }
}
