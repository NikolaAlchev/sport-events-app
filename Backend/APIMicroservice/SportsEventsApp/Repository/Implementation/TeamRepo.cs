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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Repository.Implementation
{
    public class TeamRepo : ITeamRepo
    {

        private readonly HttpClient _httpClient;

        public TeamRepo()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "c516a9286e324f82a787a9fdd9afbbe9");
        }

        public async Task<List<BasicPlayerInfoDTO>> GetSquadFromTeamId(int id)
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
                        var DTO = new BasicPlayerInfoDTO
                        {
                            Name = player.GetProperty("name").ToString(),
                            Id = player.GetProperty("id").GetInt32(),
                            Position = player.GetProperty("position").ToString(),
                            Nationality = player.GetProperty("nationality").ToString(),
                        };
                        res.Add(DTO);
                    }
                    var CoachDTO = new BasicPlayerInfoDTO
                    {
                        Id = jsonDocument.RootElement.GetProperty("coach").GetProperty("id").GetInt32(),
                        Name = jsonDocument.RootElement.GetProperty("coach").GetProperty("name").ToString(),
                        Position = "Coach",
                        Nationality = jsonDocument.RootElement.GetProperty("coach").GetProperty("nationality").ToString(),
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

        public async Task<TeamInfoDTO> GetTeamById(int id)
        {

            try
            {
                TeamInfoDTO team = await getBasicTeamInfo(id);

                await setPastAndFutureMatches(id, team);

                return team;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private async Task<TeamInfoDTO> getBasicTeamInfo(int id)
        {

            TeamInfoDTO team = new TeamInfoDTO();

            string url = $"http://api.football-data.org/v4/teams/{id}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);

                    team.Name = jsonDocument.RootElement.GetProperty("name").GetString();
                    team.Venue = jsonDocument.RootElement.GetProperty("venue").GetString();
                    team.Crest = jsonDocument.RootElement.GetProperty("crest").GetString();


                    var squadArray = jsonDocument.RootElement.GetProperty("squad");

                    var result = new List<BasicPlayerInfoDTO>();
                    foreach (var player in squadArray.EnumerateArray())
                    {
                        var DTO = new BasicPlayerInfoDTO
                        {
                            Name = player.GetProperty("name").ToString(),
                            Id = player.GetProperty("id").GetInt32(),
                            Position = player.GetProperty("position").ToString(),
                            Nationality = player.GetProperty("nationality").ToString(),
                        };
                        result.Add(DTO);
                    }
                    var CoachDTO = new BasicPlayerInfoDTO
                    {
                        Id = jsonDocument.RootElement.GetProperty("coach").GetProperty("id").GetInt32(),
                        Name = jsonDocument.RootElement.GetProperty("coach").GetProperty("name").ToString(),
                        Position = "Coach",
                        Nationality = jsonDocument.RootElement.GetProperty("coach").GetProperty("nationality").ToString(),
                    };
                    result.Add(CoachDTO);

                    team.Squad = result;
                    return team;
                }
                else
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private async Task setPastAndFutureMatches(int id, TeamInfoDTO team)
        {
            DateTime now = DateTime.Now;
            string urlPastMatches = $"https://api.football-data.org/v4/teams/{id}/matches?limit=5&dateFrom={now.AddMonths(-2).ToString("yyyy-MM-dd")}&dateTo={now.ToString("yyyy-MM-dd")}";
            string urlFutureMatches = $"https://api.football-data.org/v4/teams/{id}/matches?limit=20&dateFrom={now.ToString("yyyy-MM-dd")}&dateTo={now.AddMonths(2).ToString("yyyy-MM-dd")}";

            try
            {
                team.PastMatches = await getLastOrNextFiveMatches(urlPastMatches);
                team.FutureMatches = await getLastOrNextFiveMatches(urlFutureMatches);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        private async Task<List<MatchDTO>> getLastOrNextFiveMatches(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var matchesJson = jsonDocument.RootElement.GetProperty("matches");

                var lastFiveMatches = matchesJson.EnumerateArray().Take(5);

                List<MatchDTO> matches = new List<MatchDTO>();
                foreach (var singleMatch in lastFiveMatches)
                {
                    var DTO = new MatchDTO();
                    DTO.HomeTeamName = singleMatch.GetProperty("homeTeam").GetProperty("name").ToString();
                    DTO.Id = singleMatch.GetProperty("id").GetInt32();
                    DTO.UtcDate = singleMatch.GetProperty("utcDate").GetDateTime();
                    DTO.Status = singleMatch.GetProperty("status").ToString();
                    DTO.HomeTeamId = singleMatch.GetProperty("homeTeam").GetProperty("id").GetInt32();
                    DTO.AwayTeamName = singleMatch.GetProperty("awayTeam").GetProperty("name").ToString();
                    DTO.AwayTeamId = singleMatch.GetProperty("awayTeam").GetProperty("id").GetInt32();
                    if (singleMatch.TryGetProperty("score", out var score) && score.TryGetProperty("fullTime", out var fullTime))
                    {
                        DTO.HomeTeamScore = fullTime.TryGetProperty("home", out var homeScore) && homeScore.ValueKind != JsonValueKind.Null
                            ? homeScore.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.AwayTeamScore = fullTime.TryGetProperty("away", out var awayScore) && awayScore.ValueKind != JsonValueKind.Null
                            ? awayScore.GetInt32()
                            : 0; // Default to 0 if null
                    }


                    DTO.CompetitionName = singleMatch.GetProperty("competition").GetProperty("name").ToString();
                    DTO.CompetitionId = singleMatch.GetProperty("competition").GetProperty("id").GetInt32();
                    DTO.Venue = singleMatch.TryGetProperty("venue", out JsonElement venue) && venue.ValueKind != JsonValueKind.Null
                                        ? venue.ToString() : null;
                    DTO.HomeTeamCrest = singleMatch.GetProperty("homeTeam").GetProperty("crest").ToString();
                    DTO.AwayTeamCrest = singleMatch.GetProperty("awayTeam").GetProperty("crest").ToString();
                    DTO.LeagueEmblem = singleMatch.GetProperty("competition").GetProperty("emblem").ToString();
                    matches.Add(DTO);
                }

                return matches;
            }
            else
            {
                // Handle non-success status codes here
                throw new Exception($"Request failed with status code: {response.StatusCode}");
            }


        }


    }
}
