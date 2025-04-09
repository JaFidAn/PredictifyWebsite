namespace Application.DTOs.Teams;

public class TeamSeasonLeagueDto
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = null!;

    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = null!;
}
