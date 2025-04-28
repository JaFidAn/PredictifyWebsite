using Domain.Entities.Common;

namespace Domain.Entities;

public class AiForecast
{
    public int Id { get; set; }

    public int ForecastId { get; set; }
    public Forecast Forecast { get; set; } = null!;

    public string PredictedOutcomeName { get; set; } = null!;
    public double Confidence { get; set; }
    public bool? IsCorrect { get; set; }
    public string ModelVersion { get; set; } 
}
