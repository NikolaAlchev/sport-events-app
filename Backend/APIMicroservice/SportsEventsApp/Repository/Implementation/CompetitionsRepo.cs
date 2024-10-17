using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Repository.Interface;

namespace Repository.Implementation
{
    public class CompetitionsRepo:ICompetitionsRepo
    {
        private readonly HttpClient _httpClient;
        public CompetitionsRepo() {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "c516a9286e324f82a787a9fdd9afbbe9");
        }

        public async Task<List<CompetitionDTO>> getAllCompetitions()
        {
            string url = $"https://api.football-data.org/v4/competitions";
           
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var competitions = jsonDocument.RootElement.GetProperty("competitions");
                    
                    var list = new List<CompetitionDTO>();
                    foreach(var comp in competitions.EnumerateArray())
                    {
                        var DTO = new CompetitionDTO {
                            Id = comp.GetProperty("id").GetInt32(),
                            Country = comp.GetProperty("area").GetProperty("name").ToString(),

                            CountryFlag = comp.GetProperty("area").GetProperty("flag").ToString(),
                            Name = comp.GetProperty("name").ToString(),

                            Emblem = comp.GetProperty("emblem").ToString(),

                            Type = comp.GetProperty("type").ToString(),
                            CurrentSeasonId = comp.GetProperty("currentSeason").GetProperty("id").GetInt32(),

                            CurrentSeasonStartDate = DateOnly.FromDateTime(comp.GetProperty("currentSeason").GetProperty("startDate").GetDateTime()),
                            CurrentSeasonEndDate = DateOnly.FromDateTime(comp.GetProperty("currentSeason").GetProperty("endDate").GetDateTime()),

                            CurrentMatchDay = comp.GetProperty("currentSeason").GetProperty("currentMatchday").GetInt32()
                        };

                        list.Add(DTO);   
                    }

                    return list;
                    
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

        public async Task<List<TeamDTO>> getAllTeamsFromCompetition(int id)
        {
            string url = $"https://api.football-data.org/v4/competitions/{id}/teams";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var teams = jsonDocument.RootElement.GetProperty("teams");

                    var list = new List<TeamDTO>();
                    foreach (var team in teams.EnumerateArray())
                    {
                        var DTO = new TeamDTO
                        {
                            Id = team.GetProperty("id").GetInt32(),
                            Name = team.GetProperty("name").ToString(),
                            Crest = team.GetProperty("crest").ToString(),
                            CurrentCompetitionId = jsonDocument.RootElement.GetProperty("competition").GetProperty("id").GetInt32()
                    };

                        list.Add(DTO);
                    }

                    return list;

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
