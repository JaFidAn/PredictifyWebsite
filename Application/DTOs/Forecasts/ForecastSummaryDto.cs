using Application.DTOs.Outcomes;

namespace Application.DTOs.Forecasts;

public class ForecastSummaryDto
{
    public int MatchId { get; set; }
    public string Team1Name { get; set; } = null!;
    public string Team2Name { get; set; } = null!;

    public string BestForecastOutcomeName { get; set; } = null!;
    public double BestConfidenceRatio { get; set; }

    public string? BestForecastOutcomeNameByAI { get; set; }
    public double? BestConfidenceRatioByAI { get; set; }

    public List<OutcomeConfidenceDto> OutcomeForecasts { get; set; } = new();
}