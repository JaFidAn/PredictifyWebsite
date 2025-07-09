namespace Application.DTOs.AiForecasts;

public class AiForecastDto
{
    public int Id { get; set; }
    public int ForecastId { get; set; }
    public int MatchId { get; set; }
    public string Team1Name { get; set; } = null!;
    public string Team2Name { get; set; } = null!;
    public double ProbabilityOfCorrectness { get; set; }
    public bool? IsActuallyCorrect { get; set; }
    public string ModelVersion { get; set; } = null!;
}
