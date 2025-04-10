namespace Domain.Entities;

public class MatchOutcome
{
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int OutcomeId { get; set; }
    public Outcome Outcome { get; set; } = null!;
}
