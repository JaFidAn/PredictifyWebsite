namespace Application.Services;

public interface IForecastAiService
{
    Task<(string OutcomeName, double Confidence)> PredictBestOutcomeAsync(int matchId);
}