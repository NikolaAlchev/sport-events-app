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

                        CurrentTeamId = jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("id").GetInt32(),

                        CurrentTeamName = jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("name").ToString(),

                        contractStart = DateOnly.FromDateTime(jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("contract").GetProperty("start").GetDateTime()),

                        contractEnd = DateOnly.FromDateTime(jsonDocument.RootElement.GetProperty("currentTeam").GetProperty("contract").GetProperty("until").GetDateTime())
                    };

                    url = $"https://api.football-data.org/v4/persons/{id}/matches";
                    HttpResponseMessage responseStats = await _httpClient.GetAsync(url);
                    if (responseStats.IsSuccessStatusCode)
                    {
                        jsonResponse = await responseStats.Content.ReadAsStringAsync();
                        jsonDocument = JsonDocument.Parse(jsonResponse);
                        var aggregations = jsonDocument.RootElement.GetProperty("aggregations");

                        DTO.MatchesOnPitch = aggregations.GetProperty("matchesOnPitch").GetInt32();
                        DTO.Starting = aggregations.GetProperty("startingXI").GetInt32();
                        DTO.MinutesPlayed = aggregations.GetProperty("minutesPlayed").GetInt32();
                        DTO.Goals = aggregations.GetProperty("goals").GetInt32();
                        DTO.OwnGoals = aggregations.GetProperty("ownGoals").GetInt32();
                        DTO.Assists = aggregations.GetProperty("assists").GetInt32();
                        DTO.Penalties = aggregations.GetProperty("penalties").GetInt32();
                        DTO.SubbedOut = aggregations.GetProperty("subbedOut").GetInt32();
                        DTO.SubbedIn = aggregations.GetProperty("subbedIn").GetInt32();
                        DTO.YellowCards = aggregations.GetProperty("yellowCards").GetInt32();
                        DTO.YellowRedCards = aggregations.GetProperty("yellowRedCards").GetInt32();
                        DTO.RedCards = aggregations.GetProperty("redCards").GetInt32();
                        return DTO;
                    }
                    else 
                    {
                        throw new Exception($"Request failed with status code: {response.StatusCode}");
                    }
                   

                    
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
