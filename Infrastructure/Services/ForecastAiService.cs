using Application.Repositories.ForecastRepositories;
using Application.Repositories.OutcomeRepositories;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLTrainer.Models;

namespace Infrastructure.Services;

public class ForecastAiService : IForecastAiService
{
    private readonly IForecastReadRepository _forecastReadRepository;
    private readonly IOutcomeReadRepository _outcomeReadRepository;
    private readonly IMapper _mapper;
    private readonly MLContext _mlContext;
    private readonly Dictionary<int, ITransformer> _models;

    public ForecastAiService(
        IForecastReadRepository forecastReadRepository,
        IOutcomeReadRepository outcomeReadRepository,
        IMapper mapper)
    {
        _forecastReadRepository = forecastReadRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _mapper = mapper;
        _mlContext = new MLContext();
        _models = new Dictionary<int, ITransformer>();

        LoadAllModels().GetAwaiter().GetResult(); // Sync call for constructor
    }

    private async Task LoadAllModels()
    {
        var modelDir = Path.Combine(AppContext.BaseDirectory, "Model");

        var outcomes = await _outcomeReadRepository.GetAll().ToListAsync();

        foreach (var outcome in outcomes)
        {
            var modelPath = Path.Combine(modelDir, $"ForecastModel_{outcome.Code}.zip");
            if (File.Exists(modelPath))
            {
                using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var model = _mlContext.Model.Load(stream, out _);
                _models[outcome.Id] = model;
            }
        }
    }

    public async Task<(string OutcomeName, double Confidence)> PredictBestOutcomeAsync(int matchId)
    {
        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
            .ToListAsync();

        var predictions = new List<(string OutcomeName, double Confidence)>();

        foreach (var forecast in forecasts)
        {
            if (!_models.TryGetValue(forecast.OutcomeId, out var model))
                continue;

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastModelInput, ForecastModelOutput>(model);

            var input = new ForecastModelInput
            {
                OutcomeId = forecast.OutcomeId,
                StreakCount = forecast.StreakCount,
                MaxStreak = forecast.MaxStreak,
                Ratio = (float)forecast.Ratio
            };

            var prediction = predictionEngine.Predict(input);
            predictions.Add((forecast.Outcome.Name, Math.Round(prediction.Probability, 3)));
        }

        var best = predictions.OrderByDescending(p => p.Confidence).FirstOrDefault();
        return best;
    }
}
