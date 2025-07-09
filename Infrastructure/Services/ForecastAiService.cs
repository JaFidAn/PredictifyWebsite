using Application.Repositories.ForecastRepositories;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLTrainer.Models;

namespace Infrastructure.Services;

public class ForecastAiService : IForecastAiService
{
    private readonly IForecastReadRepository _forecastReadRepository;
    private readonly IMapper _mapper;
    private readonly MLContext _mlContext;
    private PredictionEngine<ForecastModelInput, ForecastModelOutput>? _predictionEngine;

    public ForecastAiService(
        IForecastReadRepository forecastReadRepository,
        IMapper mapper)
    {
        _forecastReadRepository = forecastReadRepository;
        _mapper = mapper;
        _mlContext = new MLContext();

        LoadSingleModel(); 
    }

    private void LoadSingleModel()
    {
        var modelDir = Path.Combine(AppContext.BaseDirectory, "Model");
        var modelPath = Path.Combine(modelDir, "ForecastPredictionModel.zip");

        if (!File.Exists(modelPath))
            throw new FileNotFoundException("Forecast ML model faylı tapılmadı.", modelPath);

        using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = _mlContext.Model.Load(stream, out _);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastModelInput, ForecastModelOutput>(model);
    }

    public async Task<(string OutcomeName, double Confidence)> PredictBestOutcomeAsync(int matchId)
    {
        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
            .ToListAsync();

        if (_predictionEngine == null || !forecasts.Any())
            return ("N/A", 0);

        var predictions = new List<(string OutcomeName, double Confidence)>();

        foreach (var forecast in forecasts)
        {
            var input = new ForecastModelInput
            {
                StreakCount = forecast.StreakCount,
                MaxStreak = forecast.MaxStreak,
                Ratio = (float)forecast.Ratio
            };

            var prediction = _predictionEngine.Predict(input);
            predictions.Add((forecast.Outcome.Name, Math.Round(prediction.Probability, 3)));
        }

        var best = predictions.OrderByDescending(p => p.Confidence).FirstOrDefault();
        return best;
    }
}
