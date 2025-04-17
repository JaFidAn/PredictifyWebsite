namespace Domain.Entities;

public class TeamOutcomeStreak
{
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int OutcomeId { get; set; }
    public Outcome Outcome { get; set; } = null!;

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int StreakCount { get; set; }
    public int MaxStreak { get; set; }
    public double Ratio { get; set; }
    public DateTime MatchDate { get; set; }
}
