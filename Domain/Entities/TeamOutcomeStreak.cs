using Domain.Entities.Common;

namespace Domain.Entities;

public class TeamOutcomeStreak : BaseEntity<int>
{
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int OutcomeId { get; set; }
    public Outcome Outcome { get; set; } = null!;

    public int StreakCount { get; set; }

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public DateTime MatchDate { get; set; }
}
