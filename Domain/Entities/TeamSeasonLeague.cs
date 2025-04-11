namespace Domain.Entities;

public class TeamSeasonLeague
{
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int LeagueId { get; set; }
    public League League { get; set; } = null!;
}
