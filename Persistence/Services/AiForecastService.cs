using Application.Core;
using Application.DTOs.AiForecasts;
using Application.Helpers;
using Application.Params;
using Application.Repositories;
using Application.Repositories.AiForecastRepositories;
using Application.Repositories.ForecastRepositories;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLTrainer.Models;

namespace Persistence.Services;

public class AiForecastService : IAiForecastService
{
    private readonly IAiForecastWriteRepository _writeRepository;
    private readonly IAiForecastReadRepository _readRepository;
    private readonly IForecastReadRepository _forecastReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MLContext _mlContext;
    private readonly PredictionEngine<ForecastModelInput, ForecastModelOutput>? _predictionEngine;

    public AiForecastService(
        IAiForecastWriteRepository writeRepository,
        IAiForecastReadRepository readRepository,
        IForecastReadRepository forecastReadRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _forecastReadRepository = forecastReadRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mlContext = new MLContext();

        _predictionEngine = LoadModel();
    }

    private PredictionEngine<ForecastModelInput, ForecastModelOutput>? LoadModel()
    {
        var modelDir = Path.Combine(AppContext.BaseDirectory, "Model");
        var modelPath = Path.Combine(modelDir, "ForecastPredictionModel.zip");

        if (!File.Exists(modelPath))
            throw new FileNotFoundException("ForecastPredictionModel.zip tapılmadı", modelPath);

        using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = _mlContext.Model.Load(stream, out _);
        return _mlContext.Model.CreatePredictionEngine<ForecastModelInput, ForecastModelOutput>(model);
    }

    public async Task<Result<AiForecastDto>> GetByForecastIdAsync(int forecastId)
    {
        var entity = await _readRepository.GetAll()
            .Where(x => x.ForecastId == forecastId)
            .ProjectTo<AiForecastDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return entity == null
            ? Result<AiForecastDto>.Failure("AI forecast not found.", 404)
            : Result<AiForecastDto>.Success(entity);
    }

    public async Task<Result<List<AiForecastDto>>> GetAllByMatchIdAsync(int matchId)
    {
        var forecastIds = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Select(x => x.Id)
            .ToListAsync();

        var aiForecasts = await _readRepository.GetAll()
            .Where(x => forecastIds.Contains(x.ForecastId))
            .ProjectTo<AiForecastDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<List<AiForecastDto>>.Success(aiForecasts);
    }

    public async Task<Result<bool>> AddAiForecastAsync(int forecastId, double probability, string modelVersion)
    {
        var forecast = await _forecastReadRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == forecastId);

        if (forecast == null)
            return Result<bool>.Failure("Forecast not found", 404);

        var aiForecast = new AiForecast
        {
            ForecastId = forecastId,
            MatchId = forecast.MatchId,
            ProbabilityOfCorrectness = probability,
            ModelVersion = modelVersion
        };

        await _writeRepository.AddAsync(aiForecast);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetIsCorrectAsync(int forecastId, bool isCorrect)
    {
        var aiForecast = await _readRepository.GetAll()
            .FirstOrDefaultAsync(x => x.ForecastId == forecastId);

        if (aiForecast == null)
            return Result<bool>.Failure("AI forecast not found", 404);

        aiForecast.IsActuallyCorrect = isCorrect;
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<double>> GetAccuracyAsync(AiForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Forecast).ThenInclude(f => f.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsActuallyCorrect.HasValue);

        if (filters.StartDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate >= filters.StartDate.Value);

        if (filters.EndDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate <= filters.EndDate.Value);

        if (filters.LeagueId.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));

        var forecasts = await query.ToListAsync();
        if (forecasts.Count == 0)
            return Result<double>.Success(0);

        var accuracy = (double)forecasts.Count(x => x.IsActuallyCorrect == true) / forecasts.Count * 100;
        return Result<double>.Success(Math.Round(accuracy, 2));
    }

    public async Task<Result<bool>> RemoveByForecastIdsAsync(List<int> forecastIds)
    {
        await _writeRepository.RemoveByForecastIdsAsync(forecastIds);
        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveAllAsync()
    {
        await _writeRepository.RemoveAllAsync();
        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<AiForecastDto>>> GetAllByMatchIdWithTeamsAsync(int matchId)
    {
        var forecasts = await _readRepository.GetAll()
            .Include(x => x.Forecast)
            .ThenInclude(f => f.Match)
            .ThenInclude(m => m.Team1)
            .Include(x => x.Forecast)
            .ThenInclude(f => f.Match)
            .ThenInclude(m => m.Team2)
            .Where(x => x.Forecast.MatchId == matchId)
            .ToListAsync();

        var result = forecasts.Select(x => new AiForecastDto
        {
            Id = x.Id,
            ForecastId = x.ForecastId,
            MatchId = x.MatchId,
            ProbabilityOfCorrectness = x.ProbabilityOfCorrectness,
            IsActuallyCorrect = x.IsActuallyCorrect,
            ModelVersion = x.ModelVersion,
            Team1Name = x.Forecast.Match.Team1?.Name ?? "Team 1",
            Team2Name = x.Forecast.Match.Team2?.Name ?? "Team 2"
        }).ToList();

        return Result<List<AiForecastDto>>.Success(result);
    }

    public async Task<AiForecastPreviewDto> PreviewForecastAsync(int matchId)
    {
        if (_predictionEngine == null)
            throw new InvalidOperationException("ML modeli yüklənməyib.");

        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Outcome)
            .ToListAsync();

        var predictions = forecasts.Select(f =>
        {
            var input = new ForecastModelInput
            {
                StreakCount = f.StreakCount,
                MaxStreak = f.MaxStreak,
                Ratio = (float)f.Ratio
            };

            var result = _predictionEngine.Predict(input);
            return new OutcomePredictionDto
            {
                Outcome = f.Outcome.Name,
                Confidence = Math.Round(result.Probability, 3)
            };
        }).ToList();

        var best = predictions.OrderByDescending(p => p.Confidence).FirstOrDefault();

        return new AiForecastPreviewDto
        {
            MatchId = matchId,
            AllOutcomePredictions = predictions,
            BestForecastOutcome = best?.Outcome ?? "N/A",
            BestConfidence = best?.Confidence ?? 0,
            ModelVersion = "v3"
        };
    }

    public async Task<Result<bool>> GenerateAiForecastForLastSeasonAsync(int seasonId)
{
    if (_predictionEngine == null)
        return Result<bool>.Failure("ML modeli yüklənməyib.", 500);

    var matchIds = await _forecastReadRepository.GetAll()
        .Where(x => x.Match.MatchTeamSeasonLeagues.Any(m => m.SeasonId == seasonId))
        .Select(x => x.MatchId)
        .Distinct()
        .ToListAsync();

    foreach (var matchId in matchIds)
    {
        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId && x.IsForecasted)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
                .ThenInclude(m => m.Outcomes) // 💡 Əsas düzəliş budur
            .ToListAsync();

        if (!forecasts.Any())
            continue;

        var predictions = forecasts.Select(f =>
        {
            var input = new ForecastModelInput
            {
                StreakCount = f.StreakCount,
                MaxStreak = f.MaxStreak,
                Ratio = (float)f.Ratio
            };

            var result = _predictionEngine.Predict(input);
            return new
            {
                Forecast = f,
                Probability = Math.Round(result.Probability, 3)
            };
        }).ToList();

        var best = predictions.OrderByDescending(p => p.Probability).FirstOrDefault();
        if (best == null)
            continue;

        var aiForecast = new AiForecast
        {
            ForecastId = best.Forecast.Id,
            MatchId = best.Forecast.MatchId,
            ProbabilityOfCorrectness = best.Probability,
            ModelVersion = "v3",
            IsActuallyCorrect = best.Forecast.Match.IsCompleted
                ? best.Forecast.Match.Outcomes.Any(o =>
                      o.TeamId == best.Forecast.TeamId &&
                      o.OutcomeId == best.Forecast.OutcomeId)
                : null
        };

        await _writeRepository.AddAsync(aiForecast);
    }

    await _unitOfWork.SaveChangesAsync();
    return Result<bool>.Success(true);
}


    public async Task<Result<PagedResult<AiForecastTrendDto>>> GetTrendReportAsync(AiForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Forecast)
            .ThenInclude(f => f.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsActuallyCorrect.HasValue);

        if (filters.StartDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate >= filters.StartDate);
        if (filters.EndDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate <= filters.EndDate);
        if (filters.LeagueId.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchTeamSeasonLeagues.Any(m => m.LeagueId == filters.LeagueId));

        var grouped = await query.GroupBy(x => x.Forecast.Match.MatchDate.Date)
            .Select(g => new AiForecastTrendDto
            {
                Date = g.Key,
                Total = g.Count(),
                Correct = g.Count(x => x.IsActuallyCorrect == true),
                Accuracy = Math.Round((double)g.Count(x => x.IsActuallyCorrect == true) / g.Count() * 100, 2)
            })
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return Result<PagedResult<AiForecastTrendDto>>.Success(
            await PagedResult<AiForecastTrendDto>.CreateFromListAsync(grouped, filters.PageNumber, filters.PageSize, CancellationToken.None)
        );
    }
}
