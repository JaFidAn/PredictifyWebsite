namespace Application.DTOs.TeamOutcomeStreaks;

public class TeamOutcomeStreakDto
{
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public int OutcomeId { get; set; }
    public string? OutcomeName { get; set; }
    public int MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public int StreakCount { get; set; }
    public int MaxStreak { get; set; }     
    public double Ratio { get; set; }   
}

