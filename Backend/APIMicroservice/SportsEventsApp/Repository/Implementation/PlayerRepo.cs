using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Repository.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Repository.Implementation
{
    public class PlayerRepo : IPlayerRepo
    {
      
        private readonly HttpClient _httpClient;

        public PlayerRepo()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "c516a9286e324f82a787a9fdd9afbbe9");
        }

        public async Task<PlayerDTO> GetPlayer(int id)
        {
            string url = $"https://api.football-data.org/v4/persons/{id}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);


                    var DTO = new PlayerDTO
                    {
                        Id = jsonDocument.RootElement.GetProperty("id").GetInt32(),

                        FirstName = jsonDocument.RootElement.GetProperty("firstName").ToString(),


                        LastName = jsonDocument.RootElement.GetProperty("lastName").ToString(),

                        DateOfBirth = DateOnly.FromDateTime(jsonDocument.RootElement.GetProperty("dateOfBirth").GetDateTime()),

                        Nationality = jsonDocument.RootElement.GetProperty("nationality").ToString(),
                        Position = jsonDocument.RootElement.GetProperty("position").ToString(),

                        ShirtNumber = jsonDocument.RootElement.GetProperty("shirtNumber").GetInt32(),

                        CurrentTeamId = jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("area").GetProperty("id").GetInt32(),

                        CurrentTeamName = jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("area").GetProperty("name").ToString(),

                        contractStart = DateOnly.Parse(jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("contract").GetProperty("start").ToString()),

                        contractEnd = DateOnly.Parse(jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("contract").GetProperty("until").ToString())
                    };

                    /*                    url = $"https://api.football-data.org/v4/persons/{id}/matches";
                                        HttpResponseMessage responseStats = await _httpClient.GetAsync(url);
                                        if (responseStats.IsSuccessStatusCode)
                                        {
                                            jsonResponse = await responseStats.Content.ReadAsStringAsync();
                                            jsonDocument = JsonDocument.Parse(jsonResponse);


                                            DTO.MatchesOnPitch = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("matchesOnPitch").GetInt32();
                                            DTO.Starting = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("startingXI").GetInt32();
                                            DTO.MinutesPlayed = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("minutesPlayed").GetInt32();
                                            DTO.Goals = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("goals").GetInt32();
                                            DTO.OwnGoals = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("ownGoals").GetInt32();
                                            DTO.Assists = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("assists").GetInt32();
                                            DTO.Penalties = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("penalties").GetInt32();
                                            DTO.SubbedOut = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("subbedOut").GetInt32();
                                            DTO.SubbedIn = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("subbedIn").GetInt32();
                                            DTO.YellowCards = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("yellowCards").GetInt32();
                                            DTO.YellowRedCards = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("yellowRedCards").GetInt32();
                                            DTO.RedCards = jsonDocument.RootElement.GetProperty("aggregations").GetProperty("redCards").GetInt32();
                                            return DTO;
                                        }*/
                    /*                    else 
                                        {
                                            throw new Exception($"Request failed with status code: {response.StatusCode}");
                                        }*/


                    return DTO;
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

        public async Task<List<TopScorerDTO>> TopScorersFromCompId(int id)
        {
            string url = $"https://api.football-data.org/v4/competitions/{id}/scorers";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var scoreresArray = jsonDocument.RootElement.GetProperty("scorers");

                    var res = new List<TopScorerDTO>(); 
                    foreach(var player in scoreresArray.EnumerateArray())
                    {
                        var DTO = new TopScorerDTO
                        {
                            Id = player.GetProperty("player").GetProperty("id").GetInt32(),
                            FirstName = player.GetProperty("player").GetProperty("firstName").ToString(),
                            LastName = player.GetProperty("player").GetProperty("lastName").ToString(),
                            Nationality = player.GetProperty("player").GetProperty("nationality").ToString(),
                            Position = player.GetProperty("player").GetProperty("position").ToString(),
                            CurrentTeamId = player.GetProperty("team").GetProperty("id").GetInt32(),
                            CurrentTeamName = player.GetProperty("team").GetProperty("name").ToString(),
                            CurrentTeamCrest = player.GetProperty("team").GetProperty("crest").ToString(),


                        };
                        
                        DTO.ShirtNumber = player.GetProperty("player").TryGetProperty("shirtNumber", out var shirtNumber) && shirtNumber.ValueKind != JsonValueKind.Null
                            ? shirtNumber.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.Goals = player.TryGetProperty("goals", out var goals) && goals.ValueKind != JsonValueKind.Null
                            ? goals.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.Assists = player.TryGetProperty("assists", out var assists) && assists.ValueKind != JsonValueKind.Null
                            ? assists.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.PlayedMatches = player.TryGetProperty("playedMatches", out var playedMatches) && playedMatches.ValueKind != JsonValueKind.Null
                            ? playedMatches.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.Penalties = player.TryGetProperty("penalties", out var penalties) && penalties.ValueKind != JsonValueKind.Null
                            ? penalties.GetInt32()
                            : 0; // Default to 0 if null

                        res.Add( DTO );
                        
                    }



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
    }
}
