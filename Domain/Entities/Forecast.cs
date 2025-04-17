using Domain.Entities.Common;

namespace Domain.Entities;

public class Forecast : BaseEntity<int>
{
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public int OutcomeId { get; set; }

    public int StreakCount { get; set; }
    public int MaxStreak { get; set; }
    public double Ratio { get; set; }

    public bool IsForecasted { get; set; } 
    public bool? IsCorrect { get; set; }   

    // Navigation Properties
    public Match Match { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public Outcome Outcome { get; set; } = null!;
}