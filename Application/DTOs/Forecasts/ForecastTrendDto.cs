namespace Application.DTOs.Forecasts;

public class ForecastTrendDto
{
    public DateTime Date { get; set; }
    public int Total { get; set; }
    public int Correct { get; set; }
    public double Accuracy { get; set; }
}