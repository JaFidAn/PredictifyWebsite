namespace Application.DTOs.Forecasts;

public class ForecastDto
{
    public int Id { get; set; }

    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public int OutcomeId { get; set; }

    public int StreakCount { get; set; }
    public int MaxStreak { get; set; }
    public double Ratio { get; set; }

    public bool IsForecasted { get; set; }
    public bool? IsCorrect { get; set; }

    // Related info for display
    public string? TeamName { get; set; }
    public string? OutcomeName { get; set; }
}