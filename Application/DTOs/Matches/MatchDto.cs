using Application.DTOs.MatchOutcomes;
using Application.DTOs.MatchTeamSeasonLeagues;

namespace Application.DTOs.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public int Team1Id { get; set; }
    public string Team1Name { get; set; } = null!;
    public int Team2Id { get; set; }
    public string Team2Name { get; set; } = null!;
    public DateTime MatchDate { get; set; }
    public int? Team1Goals { get; set; }
    public int? Team2Goals { get; set; }
    public bool IsCompleted { get; set; }
    public List<MatchOutcomeDto> Outcomes { get; set; } = new();
    public List<MatchTeamSeasonLeagueDto> TeamSeasonLeagues { get; set; } = new();
}
