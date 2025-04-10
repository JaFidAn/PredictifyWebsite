namespace Application.DTOs.MatchTeamSeasonLeagues;

public class MatchTeamSeasonLeagueDto
{
    public int TeamId { get; set; }
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = null!;
    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = null!;
}
