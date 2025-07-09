namespace Application.DTOs.AiForecasts;

public class ForecastComparisonDto
{
    public int MatchId { get; set; }
    public string Team1Name { get; set; } = null!;
    public string Team2Name { get; set; } = null!;
    
    public string? ClassicForecastOutcome { get; set; }
    public bool? ClassicIsCorrect { get; set; }

    public string? AiForecastOutcome { get; set; }
    public bool? AiIsCorrect { get; set; }
}