namespace Application.DTOs.AiForecasts;

public class AiForecastTrendDto
{
    public DateTime Date { get; set; }           
    public int Total { get; set; }               
    public int Correct { get; set; }             
    public double Accuracy { get; set; }         
}