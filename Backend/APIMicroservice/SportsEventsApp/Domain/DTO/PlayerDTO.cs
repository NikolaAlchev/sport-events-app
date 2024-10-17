using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class PlayerDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Nationality { get; set; }
        public string Position { get; set; }

        public int ShirtNumber { get; set; }

        public int CurrentTeamId { get; set;}
    
        public string CurrentTeamName { get; set; }

        public DateOnly contractStart { get; set; }

        public DateOnly contractEnd { get; set; }

        public int MatchesOnPitch { get; set; }

        public int Starting { get; set;}

        public int MinutesPlayed { get; set; }

        public int Goals {  get; set; }

        public int OwnGoals { get; set; }

        public int Assists {  get; set; }

        public int Penalties { get; set; }

        public int SubbedOut {  get; set; }

        public int SubbedIn { get; set; }

        public int YellowCards { get; set; }    

        public int YellowRedCards { get; set; }

        public int RedCards { get; set; }

    }
}
