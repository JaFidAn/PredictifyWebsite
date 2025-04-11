using Domain.Entities.Common;

namespace Domain.Entities;

public class Match : BaseEntity<int>
{
    public int Team1Id { get; set; }
    public Team Team1 { get; set; } = null!;

    public int Team2Id { get; set; }
    public Team Team2 { get; set; } = null!;

    public DateTime MatchDate { get; set; }

    public int? Team1Goals { get; set; }
    public int? Team2Goals { get; set; }

    public bool IsCompleted { get; set; }

    public ICollection<MatchTeamSeasonLeague> MatchTeamSeasonLeagues { get; set; } = new List<MatchTeamSeasonLeague>();
    public ICollection<MatchOutcome> Outcomes { get; set; } = new List<MatchOutcome>();
}
