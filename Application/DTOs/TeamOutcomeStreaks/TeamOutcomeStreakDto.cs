namespace Application.DTOs.TeamOutcomeStreaks;

public class TeamOutcomeStreakDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public int OutcomeId { get; set; }
    public string? OutcomeName { get; set; }
    public int MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public int StreakCount { get; set; }
}
