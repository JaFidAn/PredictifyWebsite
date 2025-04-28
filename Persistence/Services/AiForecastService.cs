using Application.Core;
using Application.DTOs.AiForecasts;
using Application.Helpers;
using Application.Params;
using Application.Repositories;
using Application.Repositories.AiForecastRepositories;
using Application.Repositories.ForecastRepositories;
using Application.Repositories.OutcomeRepositories;
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
    private readonly IOutcomeReadRepository _outcomeReadRepository;
    private readonly Dictionary<int, ITransformer> _models;
    private readonly MLContext _mlContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AiForecastService(
        IAiForecastWriteRepository writeRepository,
        IAiForecastReadRepository readRepository,
        IForecastReadRepository forecastReadRepository,
        IOutcomeReadRepository outcomeReadRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _forecastReadRepository = forecastReadRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mlContext = new MLContext();
        _models = new Dictionary<int, ITransformer>();
        LoadAllModels().GetAwaiter().GetResult();
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

    public async Task<Result<bool>> AddAiForecastAsync(int forecastId, string predictedOutcomeName, double confidence, string modelVersion)
    {
        var forecast = await _forecastReadRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == forecastId);

        if (forecast == null)
            return Result<bool>.Failure("Forecast not found", 404);

        var aiForecast = new AiForecast
        {
            ForecastId = forecastId,
            PredictedOutcomeName = predictedOutcomeName,
            Confidence = confidence,
            ModelVersion = modelVersion
        };

        await _writeRepository.AddAsync(aiForecast);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetIsCorrectAsync(int forecastId, bool isCorrect)
    {
        var aiForecast = await _readRepository.GetAll()
            .Where(x => x.ForecastId == forecastId)
            .FirstOrDefaultAsync();

        if (aiForecast == null)
            return Result<bool>.Failure("AI forecast not found", 404);

        aiForecast.IsCorrect = isCorrect;
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<double>> GetAccuracyAsync(AiForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Forecast)
            .ThenInclude(f => f.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsCorrect.HasValue);

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

        int correctCount = forecasts.Count(x => x.IsCorrect == true);
        double accuracy = (double)correctCount / forecasts.Count * 100;

        return Result<double>.Success(Math.Round(accuracy, 2));
    }

    public async Task<AiForecastPreviewDto> PreviewForecastAsync(int matchId)
    {
        var forecasts = await _forecastReadRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
            .ToListAsync();

        var allPredictions = new List<OutcomePredictionDto>();
        string bestOutcome = "N/A";
        double bestConfidence = 0;

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
            var confidence = Math.Round(prediction.Probability, 3);

            allPredictions.Add(new OutcomePredictionDto
            {
                Outcome = forecast.Outcome.Name,
                Confidence = confidence
            });

            if (confidence > bestConfidence)
            {
                bestConfidence = confidence;
                bestOutcome = forecast.Outcome.Name;
            }
        }

        return new AiForecastPreviewDto
        {
            MatchId = matchId,
            AllOutcomePredictions = allPredictions.OrderByDescending(x => x.Confidence).ToList(),
            BestForecastOutcome = bestOutcome,
            BestConfidence = bestConfidence,
            ModelVersion = "v3-" + bestOutcome.ToUpper().Replace(" ", "_")
        };
    }

    public async Task<Result<bool>> GenerateAiForecastForLastSeasonAsync(int seasonId)
    {
        var matchIds = await _forecastReadRepository.GetAll()
            .Where(x => x.Match.MatchTeamSeasonLeagues.Any(mtsl => mtsl.SeasonId == seasonId))
            .Select(x => x.MatchId)
            .Distinct()
            .ToListAsync();

        foreach (var matchId in matchIds)
        {
            var aiPreview = await PreviewForecastAsync(matchId);

            if (aiPreview != null && aiPreview.AllOutcomePredictions.Any())
            {
                var bestForecast = await _forecastReadRepository.GetAll()
                    .Where(x => x.MatchId == matchId)
                    .Include(x => x.Outcome)
                    .Include(x => x.Match)
                    .FirstOrDefaultAsync(x => x.Outcome.Name == aiPreview.BestForecastOutcome);

                if (bestForecast != null)
                {
                    await AddAiForecastAsync(
                        bestForecast.Id,
                        aiPreview.BestForecastOutcome,
                        aiPreview.BestConfidence,
                        aiPreview.ModelVersion
                    );

                    if (bestForecast.Match.IsCompleted)
                    {
                        bool isCorrect = ForecastHelper.CheckIsCorrect(bestForecast.Match, aiPreview.BestForecastOutcome);

                        var aiForecast = await _readRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.ForecastId == bestForecast.Id);

                        if (aiForecast != null)
                        {
                            aiForecast.IsCorrect = isCorrect;
                        }
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }


    public async Task<Result<PagedResult<ForecastComparisonDto>>> GetForecastComparisonAsync(AiForecastFilterParams filters)
    {
        var aiForecasts = await _readRepository.GetAll()
            .Include(x => x.Forecast)
                .ThenInclude(f => f.Match)
                    .ThenInclude(m => m.Team1)
            .Include(x => x.Forecast)
                .ThenInclude(f => f.Match)
                    .ThenInclude(m => m.Team2)
            .Include(x => x.Forecast)
                .ThenInclude(f => f.Match)
                    .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .ToListAsync();

        var classicForecasts = await _forecastReadRepository.GetAll()
            .Include(x => x.Outcome)
            .Where(x => x.IsForecasted)
            .GroupBy(x => x.MatchId)
            .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(f => f.Ratio).FirstOrDefault());

        var grouped = aiForecasts.GroupBy(x => x.Forecast.MatchId).ToList();
        var result = new List<ForecastComparisonDto>();

        foreach (var group in grouped)
        {
            var match = group.First().Forecast.Match;
            var matchId = match.Id;

            if (filters.LeagueId.HasValue && !match.MatchTeamSeasonLeagues.Any(x => x.LeagueId == filters.LeagueId.Value))
                continue;

            if (filters.StartDate.HasValue && match.MatchDate < filters.StartDate.Value)
                continue;

            if (filters.EndDate.HasValue && match.MatchDate > filters.EndDate.Value)
                continue;

            classicForecasts.TryGetValue(matchId, out var bestClassic);

            var ai = group.FirstOrDefault(x => x.ForecastId == bestClassic?.Id)
                     ?? group.FirstOrDefault();

            result.Add(new ForecastComparisonDto
            {
                MatchId = matchId,
                Team1Name = match.Team1.Name,
                Team2Name = match.Team2.Name,
                ClassicForecastOutcome = bestClassic?.Outcome?.Name,
                ClassicIsCorrect = bestClassic?.IsCorrect,
                AiForecastOutcome = ai?.PredictedOutcomeName,
                AiIsCorrect = ai?.IsCorrect
            });
        }

        var pagedResult = await PagedResult<ForecastComparisonDto>.CreateFromListAsync(
            result, filters.PageNumber, filters.PageSize, CancellationToken.None);

        return Result<PagedResult<ForecastComparisonDto>>.Success(pagedResult);
    }

    public async Task<Result<PagedResult<AiForecastTrendDto>>> GetTrendReportAsync(AiForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Forecast)
            .ThenInclude(f => f.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsCorrect.HasValue);

        if (filters.StartDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate >= filters.StartDate.Value);

        if (filters.EndDate.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchDate <= filters.EndDate.Value);

        if (filters.LeagueId.HasValue)
            query = query.Where(x => x.Forecast.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));

        var groupedQuery = query
            .GroupBy(x => x.Forecast.Match.MatchDate.Date)
            .Select(g => new AiForecastTrendDto
            {
                Date = g.Key,
                Total = g.Count(),
                Correct = g.Count(x => x.IsCorrect == true),
                Accuracy = Math.Round((double)g.Count(x => x.IsCorrect == true) / g.Count() * 100, 2)
            })
            .OrderByDescending(x => x.Date);

        var pagedResult = await PagedResult<AiForecastTrendDto>.CreateAsync(
            groupedQuery,
            filters.PageNumber,
            filters.PageSize,
            CancellationToken.None);

        return Result<PagedResult<AiForecastTrendDto>>.Success(pagedResult);
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
            PredictedOutcomeName = x.PredictedOutcomeName,
            Confidence = x.Confidence,
            IsCorrect = x.IsCorrect,
            ModelVersion = x.ModelVersion,
            Team1Name = x.Forecast.Match.Team1?.Name ?? "Team 1",
            Team2Name = x.Forecast.Match.Team2?.Name ?? "Team 2"
        }).ToList();

        return Result<List<AiForecastDto>>.Success(result);
    }

} 