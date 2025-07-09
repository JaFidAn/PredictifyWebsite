using Domain.Entities.Common;

namespace Domain.Entities;

public class AiForecast
{
    public int Id { get; set; }
    public int ForecastId { get; set; }
    public Forecast Forecast { get; set; } = null!;
    public int MatchId { get; set; }   
    public double ProbabilityOfCorrectness { get; set; }  
    public bool? IsActuallyCorrect { get; set; } 
    public string ModelVersion { get; set; } = null!;
}
