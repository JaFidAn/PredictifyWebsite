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
    private readonly ITransformer _model;
    private readonly PredictionEngine<ForecastModelInput, ForecastModelOutput> _predictionEngine;

    public ForecastAiService(IForecastReadRepository forecastReadRepository, IMapper mapper)
    {
        _forecastReadRepository = forecastReadRepository;
        _mapper = mapper;
        _mlContext = new MLContext();

        var modelPath = Path.Combine(AppContext.BaseDirectory, "Model", "ForecastPredictionModel.zip");

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException("ML.NET model file not found.", modelPath);
        }

        using var fileStream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        _model = _mlContext.Model.Load(fileStream, out _);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastModelInput, ForecastModelOutput>(_model);
    }

    public async Task<(string OutcomeName, double Confidence)> PredictBestOutcomeAsync(int matchId)
    {
        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId && !x.IsDeleted)
            .Include(x => x.Outcome)
            .ToListAsync();

        var best = forecasts
            .Select(f =>
            {
                var input = new ForecastModelInput
                {
                    OutcomeId = f.OutcomeId,
                    StreakCount = f.StreakCount,
                    MaxStreak = f.MaxStreak,
                    Ratio = (float)f.Ratio
                };

                var prediction = _predictionEngine.Predict(input);
                return new
                {
                    f.Outcome.Name,
                    Probability = prediction.Probability
                };
            })
            .OrderByDescending(x => x.Probability)
            .FirstOrDefault();

        return best != null
            ? (best.Name, Math.Round(best.Probability, 3))
            : ("No prediction", 0);
    }
}
