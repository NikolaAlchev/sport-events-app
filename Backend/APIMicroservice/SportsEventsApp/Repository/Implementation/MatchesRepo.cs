using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Repository.Interface;

namespace Repository.Implementation
{
    public class MatchesRepo : IMatchesRepo
    {
        private readonly HttpClient _httpClient;

        public MatchesRepo()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "c516a9286e324f82a787a9fdd9afbbe9"); 
        }

        public async Task<List<MatchDTO>> getAllMatchesFromDate(DateOnly dateFrom)
        {
            string dateFromString = dateFrom.ToString("yyyy-MM-dd");
            string dateToString = dateFrom.AddDays(1).ToString("yyyy-MM-dd");
            string url = $"https://api.football-data.org/v4/matches?dateFrom={dateFromString}&dateTo={dateToString}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var matchesJson = jsonDocument.RootElement.GetProperty("matches");
                    List<MatchDTO> matches = new List<MatchDTO>();
                    foreach (var singleMatch in matchesJson.EnumerateArray())
                    {
                        var DTO = new MatchDTO();
                        DTO.HomeTeamName = singleMatch.GetProperty("homeTeam").GetProperty("name").ToString();
                        DTO.Id = singleMatch.GetProperty("id").GetInt32();
                        DTO.UtcDate = singleMatch.GetProperty("utcDate").GetDateTime();
                        DTO.Status= singleMatch.GetProperty("status").ToString();
                        DTO.HomeTeamId= singleMatch.GetProperty("homeTeam").GetProperty("id").GetInt32();
                        DTO.AwayTeamName= singleMatch.GetProperty("awayTeam").GetProperty("name").ToString();
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
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<MatchDTO> getMatchById(int id)
        {
            string url = $"https://api.football-data.org/v4/matches/{id}";
            
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    
                    
                    var DTO = new MatchDTO();
                    DTO.HomeTeamName = jsonDocument.RootElement.GetProperty("homeTeam").GetProperty("name").ToString();
                    DTO.Id = jsonDocument.RootElement.GetProperty("id").GetInt32();
                    DTO.UtcDate = jsonDocument.RootElement.GetProperty("utcDate").GetDateTime();
                    DTO.Status = jsonDocument.RootElement.GetProperty("status").ToString();
                    DTO.HomeTeamId = jsonDocument.RootElement.GetProperty("homeTeam").GetProperty("id").GetInt32();
                    DTO.AwayTeamName = jsonDocument.RootElement.GetProperty("awayTeam").GetProperty("name").ToString();
                    DTO.AwayTeamId = jsonDocument.RootElement.GetProperty("awayTeam").GetProperty("id").GetInt32();
                    if (jsonDocument.RootElement.TryGetProperty("score", out var score) && score.TryGetProperty("fullTime", out var fullTime))
                    {
                        DTO.HomeTeamScore = fullTime.TryGetProperty("home", out var homeScore) && homeScore.ValueKind != JsonValueKind.Null
                            ? homeScore.GetInt32()
                            : 0; // Default to 0 if null
                        DTO.AwayTeamScore = fullTime.TryGetProperty("away", out var awayScore) && awayScore.ValueKind != JsonValueKind.Null
                            ? awayScore.GetInt32()
                            : 0; // Default to 0 if null
                    }
                    DTO.CompetitionName = jsonDocument.RootElement.GetProperty("competition").GetProperty("name").ToString();
                    DTO.CompetitionId = jsonDocument.RootElement.GetProperty("competition").GetProperty("id").GetInt32();
                    DTO.Venue = jsonDocument.RootElement.TryGetProperty("venue", out JsonElement venue) && venue.ValueKind != JsonValueKind.Null
                    ? venue.ToString() : null;
                    DTO.HomeTeamCrest = jsonDocument.RootElement.GetProperty("homeTeam").GetProperty("crest").ToString();
                    DTO.AwayTeamCrest = jsonDocument.RootElement.GetProperty("awayTeam").GetProperty("crest").ToString();

                    var refereesJson = jsonDocument.RootElement.GetProperty("referees");

                    List<RefereeDTO> referees = new List<RefereeDTO>();

                    foreach(var referee in refereesJson.EnumerateArray())
                    {
                        RefereeDTO r = new RefereeDTO
                        {
                            Name = referee.GetProperty("name").ToString(),
                            Nationality = referee.GetProperty("nationality").ToString()
                        };
                        referees.Add(r);
                    }
                    
                    DTO.Referees = referees;

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


        public async Task<StandingsTableDTO> getStandingsTable(int competitionId)
        {
            string url = $"https://api.football-data.org/v4/competitions/{competitionId}/standings";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var standingsArray = jsonDocument.RootElement.GetProperty("standings");


                    List<TeamPositionDTO> teams = new List<TeamPositionDTO>();
                    foreach(var standing in standingsArray.EnumerateArray())
                    {
                        foreach (var team in standing.GetProperty("table").EnumerateArray())
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
                            teams.Add(DTO);
                        }

                    }

                    var res = new StandingsTableDTO
                    {
                        teamStandings = teams,
                        CompetitionName = jsonDocument.RootElement.GetProperty("competition").GetProperty("name").ToString(),
                        CompetitionId = jsonDocument.RootElement.GetProperty("competition").GetProperty("id").GetInt32()
                    };

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



        public async Task<List<MatchDTO>> getHead2Head(int id)
        {
            string url = $"https://api.football-data.org/v4/matches/{id}/head2head?limit=20";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var matchesJson = jsonDocument.RootElement.GetProperty("matches");

                List<MatchDTO> matches = new List<MatchDTO>();
                foreach (var singleMatch in matchesJson.EnumerateArray())
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
