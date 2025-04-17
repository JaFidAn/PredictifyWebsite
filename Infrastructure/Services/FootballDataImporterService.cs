using Application.Configurations;
using Application.DTOs.Import;
using Application.DTOs.Matches;
using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Persistence.Contexts;

namespace Infrastructure.Services;

public class FootballDataImporterService : IFootballDataImporterService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FootballDataImporterService> _logger;
    private readonly string _folderPath;
    private readonly IMatchService _matchService;

    public FootballDataImporterService(
        ApplicationDbContext context,
        ILogger<FootballDataImporterService> logger,
        IOptions<DataSettings> dataSettings,
        IMatchService matchService)
    {
        _context = context;
        _logger = logger;
        _folderPath = dataSettings.Value.ExternalFolderPath;
        _matchService = matchService;
    }

    public async Task ImportInitialDataAsync(CancellationToken cancellationToken)
    {
        var seasonFolders = Directory.GetDirectories(_folderPath);

        foreach (var seasonFolder in seasonFolders)
        {
            var seasonName = Path.GetFileName(seasonFolder);
            var season = await GetOrCreateSeasonAsync(seasonName, cancellationToken);

            var jsonFiles = Directory.GetFiles(seasonFolder, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var parts = fileName.Split('-');
                if (parts.Length < 2) continue;

                var countryName = parts[0].Trim();
                var leagueName = parts[1].Trim();

                var country = await GetOrCreateCountryAsync(countryName, cancellationToken);
                var league = await GetOrCreateLeagueAsync(leagueName, country.Id, cancellationToken);

                var json = await File.ReadAllTextAsync(filePath, cancellationToken);

                var leagueFileOld = JsonConvert.DeserializeObject<LeagueFileOldFormat>(json);
                if (leagueFileOld?.Rounds?.Any() == true)
                {
                    foreach (var round in leagueFileOld.Rounds)
                    {
                        foreach (var match in round.Matches)
                        {
                            var team1 = await GetOrCreateTeamAsync(match.Team1, country.Id, cancellationToken);
                            var team2 = await GetOrCreateTeamAsync(match.Team2, country.Id, cancellationToken);

                            await AddTeamSeasonLeagueAsync(team1.Id, season.Id, league.Id, cancellationToken);
                            await AddTeamSeasonLeagueAsync(team2.Id, season.Id, league.Id, cancellationToken);
                        }
                    }
                    continue;
                }

                var leagueFileNew = JsonConvert.DeserializeObject<LeagueFileNewFormat>(json);
                if (leagueFileNew?.Matches?.Any() == true)
                {
                    foreach (var match in leagueFileNew.Matches)
                    {
                        var team1 = await GetOrCreateTeamAsync(match.Team1, country.Id, cancellationToken);
                        var team2 = await GetOrCreateTeamAsync(match.Team2, country.Id, cancellationToken);

                        await AddTeamSeasonLeagueAsync(team1.Id, season.Id, league.Id, cancellationToken);
                        await AddTeamSeasonLeagueAsync(team2.Id, season.Id, league.Id, cancellationToken);
                    }
                    continue;
                }

                _logger.LogWarning("File format not recognized: {FilePath}", filePath);
            }
        }
    }

    public async Task ImportMatchesAsync(CancellationToken cancellationToken)
    {
        var seasonFolders = Directory.GetDirectories(_folderPath);

        foreach (var seasonFolder in seasonFolders)
        {
            var seasonName = Path.GetFileName(seasonFolder);
            var season = await GetOrCreateSeasonAsync(seasonName, cancellationToken);

            var jsonFiles = Directory.GetFiles(seasonFolder, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var parts = fileName.Split('-');
                if (parts.Length < 2) continue;

                var countryName = parts[0].Trim();
                var leagueName = parts[1].Trim();

                var country = await GetOrCreateCountryAsync(countryName, cancellationToken);
                var league = await GetOrCreateLeagueAsync(leagueName, country.Id, cancellationToken);

                var json = await File.ReadAllTextAsync(filePath, cancellationToken);

                var leagueFileOld = JsonConvert.DeserializeObject<LeagueFileOldFormat>(json);
                if (leagueFileOld?.Rounds?.Any() == true)
                {
                    foreach (var round in leagueFileOld.Rounds)
                    {
                        foreach (var match in round.Matches)
                        {
                            var team1 = await GetOrCreateTeamAsync(match.Team1, country.Id, cancellationToken);
                            var team2 = await GetOrCreateTeamAsync(match.Team2, country.Id, cancellationToken);

                            await AddTeamSeasonLeagueAsync(team1.Id, season.Id, league.Id, cancellationToken);
                            await AddTeamSeasonLeagueAsync(team2.Id, season.Id, league.Id, cancellationToken);

                            await AddMatchAsync(team1.Id, team2.Id, match.Date, match.Score.Ft, season.Id, league.Id, cancellationToken);
                        }
                    }
                    continue;
                }

                var leagueFileNew = JsonConvert.DeserializeObject<LeagueFileNewFormat>(json);
                if (leagueFileNew?.Matches?.Any() == true)
                {
                    foreach (var match in leagueFileNew.Matches)
                    {
                        var team1 = await GetOrCreateTeamAsync(match.Team1, country.Id, cancellationToken);
                        var team2 = await GetOrCreateTeamAsync(match.Team2, country.Id, cancellationToken);

                        await AddTeamSeasonLeagueAsync(team1.Id, season.Id, league.Id, cancellationToken);
                        await AddTeamSeasonLeagueAsync(team2.Id, season.Id, league.Id, cancellationToken);

                        await AddMatchAsync(team1.Id, team2.Id, match.Date, match.Score.Ft, season.Id, league.Id, cancellationToken);
                    }
                    continue;
                }

                _logger.LogWarning("File format not recognized: {FilePath}", filePath);
            }
        }
    }

    private async Task<Country> GetOrCreateCountryAsync(string name, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (country != null) return country;

        country = new Country { Name = name, Code = name[..Math.Min(3, name.Length)].ToUpper() };
        _context.Countries.Add(country);
        await _context.SaveChangesAsync(cancellationToken);
        return country;
    }

    private async Task<League> GetOrCreateLeagueAsync(string name, int countryId, CancellationToken cancellationToken)
    {
        var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Name == name && x.CountryId == countryId, cancellationToken);
        if (league != null) return league;

        var competition = new Competition
        {
            Name = name,
            IsInternational = false
        };
        _context.Competitions.Add(competition);
        await _context.SaveChangesAsync(cancellationToken);

        league = new League { Name = name, CountryId = countryId, CompetitionId = competition.Id };
        _context.Leagues.Add(league);
        await _context.SaveChangesAsync(cancellationToken);
        return league;
    }

    private async Task<Season> GetOrCreateSeasonAsync(string seasonName, CancellationToken cancellationToken)
    {
        var season = await _context.Seasons.FirstOrDefaultAsync(x => x.Name == seasonName, cancellationToken);
        if (season != null) return season;

        var startYear = int.Parse(seasonName.Split('-')[0]);
        season = new Season
        {
            Name = seasonName,
            StartDate = new DateTime(startYear, 8, 5),
            EndDate = new DateTime(startYear + 1, 8, 3)
        };

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync(cancellationToken);
        return season;
    }

    private async Task<Team> GetOrCreateTeamAsync(string name, int countryId, CancellationToken cancellationToken)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (team != null) return team;

        team = new Team { Name = name, CountryId = countryId };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync(cancellationToken);
        return team;
    }

    private async Task AddTeamSeasonLeagueAsync(int teamId, int seasonId, int leagueId, CancellationToken cancellationToken)
    {
        var exists = await _context.TeamSeasonLeagues
            .AnyAsync(x => x.TeamId == teamId && x.SeasonId == seasonId && x.LeagueId == leagueId, cancellationToken);

        if (!exists)
        {
            var tsl = new TeamSeasonLeague
            {
                TeamId = teamId,
                SeasonId = seasonId,
                LeagueId = leagueId
            };

            _context.TeamSeasonLeagues.Add(tsl);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task AddMatchAsync(int team1Id, int team2Id, string date, List<int> score, int seasonId, int leagueId, CancellationToken cancellationToken)
    {
        var createMatchDto = new CreateMatchDto
        {
            Team1Id = team1Id,
            Team2Id = team2Id,
            MatchDate = DateTime.Parse(date),
            Team1Goals = score.ElementAtOrDefault(0),
            Team2Goals = score.ElementAtOrDefault(1),
            SeasonId = seasonId,
            LeagueId = leagueId
        };

        var result = await _matchService.CreateWithoutForecastAsync(createMatchDto);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Match creation failed: {Reason}", result.Message);
        }
    }

}
