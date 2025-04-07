namespace Domain.Entities;

public class TeamSeasonLeague
{
    public string TeamId { get; set; } = null!;
    public Team Team { get; set; } = null!;

    public string SeasonId { get; set; } = null!;
    public Season Season { get; set; } = null!;

    public string LeagueId { get; set; } = null!;
    public League League { get; set; } = null!;
}
