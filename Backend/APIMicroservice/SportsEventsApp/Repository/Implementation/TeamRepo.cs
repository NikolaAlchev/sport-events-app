using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Domain.DTO;
using Repository.Interface;

namespace Repository.Implementation
{
    public class TeamRepo:ITeamRepo
    {
            
        private readonly HttpClient _httpClient;

        public TeamRepo()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "c516a9286e324f82a787a9fdd9afbbe9");
        }

        public async  Task<List<BasicPlayerInfoDTO>> GetSquadFromTeamId(int id)
        {
            string url = $"http://api.football-data.org/v4/teams/{id}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var squadArray = jsonDocument.RootElement.GetProperty("squad");

                    var res = new List<BasicPlayerInfoDTO>();
                    foreach (var player in squadArray.EnumerateArray())
                    {
                        var DTO = new BasicPlayerInfoDTO { 
                            Name = player.GetProperty("name").ToString(),
                            Id = player.GetProperty("id").GetInt32(),
                            Position = player.GetProperty("position").ToString()
                        };
                        res.Add(DTO);  
                    }
                    var CoachDTO = new BasicPlayerInfoDTO {
                        Id = jsonDocument.RootElement.GetProperty("coach").GetProperty("id").GetInt32(),
                        Name = jsonDocument.RootElement.GetProperty("coach").GetProperty("name").ToString(),
                        Position = "Coach"
                    };
                    res.Add(CoachDTO);

                    return res;
                }
                else
                {
                    // Handle non-success status codes here
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<TeamPositionDTO> GetTeamByCompetitionIdAndName(int id, string name)
        {
            string url = $"https://api.football-data.org/v4/competitions/{id}/standings";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var standingsArray = jsonDocument.RootElement.GetProperty("standings");

                   
                    foreach (var standing in standingsArray.EnumerateArray())
                    {
                        foreach (var team in standing.GetProperty("table").EnumerateArray())
                        {
                            if (team.GetProperty("team").GetProperty("name").ToString() == name)
                            {
                                var DTO = new TeamPositionDTO
                                {
                                    Position = team.GetProperty("position").GetInt32(),
                                    TeamName = team.GetProperty("team").GetProperty("name").ToString(),
                                    TeamId = team.GetProperty("team").GetProperty("id").GetInt32(),
                                    TeamCrest = team.GetProperty("team").GetProperty("crest").ToString(),
                                    PlayedGames = team.GetProperty("playedGames").GetInt32(),
                                    Wins = team.GetProperty("won").GetInt32(),
                                    Losses = team.GetProperty("lost").GetInt32(),
                                    Draws = team.GetProperty("draw").GetInt32(),
                                    Points = team.GetProperty("points").GetInt32(),
                                    GoalsFor = team.GetProperty("goalsFor").GetInt32(),
                                    GoalsAgainst = team.GetProperty("goalsAgainst").GetInt32()
                                };
                                return DTO;
                            }
                           
                          
                        }

                    }


                    throw new Exception($"the team by name:{name} isn't in this competition");
                }
                else
                {
                    // Handle non-success status codes here
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
