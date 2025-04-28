namespace Application.DTOs.AiForecasts;

public class AiForecastDto
{
    public int Id { get; set; }
    public int ForecastId { get; set; }
    public string Team1Name { get; set; } = null!;
    public string Team2Name { get; set; } = null!;
    public string PredictedOutcomeName { get; set; } = null!;
    public double Confidence { get; set; }
    public bool? IsCorrect { get; set; }
    public string ModelVersion { get; set; } = null!;
}