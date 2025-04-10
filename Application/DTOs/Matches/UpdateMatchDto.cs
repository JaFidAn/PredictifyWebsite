using Application.DTOs.MatchTeamSeasonLeagues;

namespace Application.DTOs.Matches;

public class UpdateMatchDto
{
    public int Id { get; set; }
    public int Team1Id { get; set; }
    public int Team2Id { get; set; }
    public DateTime MatchDate { get; set; }
    public int? Team1Goals { get; set; }
    public int? Team2Goals { get; set; }

    public int SeasonId { get; set; }
    public int LeagueId { get; set; }
}
