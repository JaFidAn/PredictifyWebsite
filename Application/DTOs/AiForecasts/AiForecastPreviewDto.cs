namespace Application.DTOs.AiForecasts;

public class AiForecastPreviewDto
{
    public int MatchId { get; set; }
    public string BestForecastOutcome { get; set; }
    public double BestConfidence { get; set; }
    public List<OutcomePredictionDto> AllOutcomePredictions { get; set; } = new();
    public string ModelVersion { get; set; }
}