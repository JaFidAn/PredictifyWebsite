using Application.Core;
using Application.DTOs.Forecasts;
using Application.DTOs.Outcomes;
using Application.Helpers;
using Application.Params;
using Application.Repositories;
using Application.Repositories.ForecastRepositories;
using Application.Repositories.MatchRepositories;
using Application.Repositories.OutcomeRepositories;
using Application.Repositories.TeamOutcomeStreakRepositories;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class ForecastService : IForecastService
{
    private readonly IMatchReadRepository _matchReadRepository;
    private readonly IForecastWriteRepository _writeRepository;
    private readonly IForecastReadRepository _readRepository;
    private readonly ITeamOutcomeStreakReadRepository _streakReadRepository;
    private readonly IOutcomeReadRepository _outcomeReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IForecastAiService _forecastAiService;
    private readonly IAiForecastService _aiForecastService;
    private readonly IMapper _mapper;

    public ForecastService(
        IMatchReadRepository matchReadRepository,
        IForecastWriteRepository writeRepository,
        IForecastReadRepository readRepository,
        ITeamOutcomeStreakReadRepository streakReadRepository,
        IOutcomeReadRepository outcomeReadRepository,
        IUnitOfWork unitOfWork,
        IForecastAiService forecastAiService,
        IAiForecastService aiForecastService,
        IMapper mapper)
    {
        _matchReadRepository = matchReadRepository;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _streakReadRepository = streakReadRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _unitOfWork = unitOfWork;
        _forecastAiService = forecastAiService;
        _aiForecastService = aiForecastService;
        _mapper = mapper;
    }

    public async Task<Result<bool>> GenerateForecastsForMatchAsync(int matchId, bool includeAi = true)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        var match = await _matchReadRepository.GetAll()
            .Include(x => x.MatchTeamSeasonLeagues)
            .Include(x => x.Outcomes)
                .ThenInclude(o => o.Outcome)
            .FirstOrDefaultAsync(x => x.Id == matchId);

        if (match == null)
            return Result<bool>.Failure("Match not found", 404);

        var existingForecasts = await _readRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .ToListAsync();

        if (existingForecasts.Any())
        {
            var forecastIds = existingForecasts.Select(x => x.Id).ToList();

            await _aiForecastService.RemoveByForecastIdsAsync(forecastIds);
            await _writeRepository.RemoveHardRangeAsync(existingForecasts);
            await _unitOfWork.SaveChangesAsync();
        }

        var teamIds = match.MatchTeamSeasonLeagues.Select(x => x.TeamId).Distinct().ToList();
        var allOutcomes = await _outcomeReadRepository.GetAll().ToListAsync();

        var latestStreaks = await _streakReadRepository.GetAll()
            .Where(x => teamIds.Contains(x.TeamId) && x.MatchDate < match.MatchDate)
            .GroupBy(x => new { x.TeamId, x.OutcomeId })
            .Select(g => g.OrderByDescending(x => x.MatchDate).First())
            .ToListAsync();

        var forecasts = latestStreaks.Select(s => new Forecast
        {
            MatchId = match.Id,
            TeamId = s.TeamId,
            OutcomeId = s.OutcomeId,
            StreakCount = s.StreakCount,
            MaxStreak = s.MaxStreak,
            Ratio = s.Ratio,
            IsForecasted = true,
            IsCorrect = match.IsCompleted
                ? match.Outcomes.Any(o => o.TeamId == s.TeamId && o.OutcomeId == s.OutcomeId)
                : null
        }).ToList();

        await _writeRepository.AddRangeAsync(forecasts);
        await _unitOfWork.SaveChangesAsync();

        if (includeAi)
        {
            var aiPreview = await _aiForecastService.PreviewForecastAsync(matchId);

            if (aiPreview != null && aiPreview.AllOutcomePredictions.Any())
            {
                var bestForecast = await _readRepository.GetAll()
                    .Where(x => x.MatchId == matchId)
                    .Include(x => x.Outcome)
                    .Include(x => x.Match)
                        .ThenInclude(m => m.Outcomes)
                            .ThenInclude(o => o.Outcome)
                    .FirstOrDefaultAsync(x => x.Outcome.Name == aiPreview.BestForecastOutcome);

                if (bestForecast != null)
                {
                    await _aiForecastService.AddAiForecastAsync(
                        bestForecast.Id,
                        aiPreview.BestForecastOutcome,
                        aiPreview.BestConfidence,
                        aiPreview.ModelVersion
                    );

                    if (bestForecast.Match.IsCompleted)
                    {
                        bool isCorrect = ForecastHelper.CheckIsCorrect(bestForecast.Match, aiPreview.BestForecastOutcome);

                        var aiForecast = await _aiForecastService.GetByForecastIdAsync(bestForecast.Id);
                        if (aiForecast.IsSuccess && aiForecast.Value is { })
                        {
                            await _aiForecastService.SetIsCorrectAsync(bestForecast.Id, isCorrect);
                        }
                    }
                }
            }
        }

        await _unitOfWork.CommitAsync();
        return Result<bool>.Success(true, "Forecasts generated for match.");
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}

public async Task<Result<bool>> GenerateAllForecastsAsync()
{
    await _unitOfWork.BeginTransactionAsync();

    try
    {
        _unitOfWork.DisableAuditLogging = true;

        await _aiForecastService.RemoveAllAsync();
        await _unitOfWork.SaveChangesAsync();

        await _writeRepository.RemoveAllAsync();
        await _unitOfWork.SaveChangesAsync();

        var matches = await _matchReadRepository.GetAll()
            .Include(x => x.MatchTeamSeasonLeagues)
            .Include(x => x.Outcomes)
            .OrderBy(x => x.MatchDate)
            .ToListAsync();

        var allOutcomes = await _outcomeReadRepository.GetAll().ToListAsync();

        var allForecasts = new List<Forecast>();

        foreach (var match in matches)
        {
            var teamIds = match.MatchTeamSeasonLeagues.Select(x => x.TeamId).Distinct().ToList();

            var latestStreaks = await _streakReadRepository.GetAll()
                .Where(x => teamIds.Contains(x.TeamId) && x.MatchDate < match.MatchDate)
                .GroupBy(x => new { x.TeamId, x.OutcomeId })
                .Select(g => g.OrderByDescending(x => x.MatchDate).First())
                .ToListAsync();

            foreach (var s in latestStreaks)
            {
                allForecasts.Add(new Forecast
                {
                    MatchId = match.Id,
                    TeamId = s.TeamId,
                    OutcomeId = s.OutcomeId,
                    StreakCount = s.StreakCount,
                    MaxStreak = s.MaxStreak,
                    Ratio = s.Ratio,
                    IsForecasted = true,
                    IsCorrect = match.IsCompleted
                        ? match.Outcomes.Any(o => o.TeamId == s.TeamId && o.OutcomeId == s.OutcomeId)
                        : null
                });
            }
        }

        await _writeRepository.AddRangeAsync(allForecasts);
        await _unitOfWork.SaveChangesAsync();

        _unitOfWork.DisableAuditLogging = false;
        await _unitOfWork.CommitAsync();

        return Result<bool>.Success(true, "All forecasts generated successfully.");
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}

    public async Task<Result<bool>> GenerateForecastsForMissingMatchesAsync()
    {
        var allMatchIds = await _matchReadRepository.GetAll()
            .Select(x => x.Id)
            .ToListAsync();

        var forecastedMatchIds = await _readRepository.GetAll()
            .Select(x => x.MatchId)
            .Distinct()
            .ToListAsync();

        var missingMatchIds = allMatchIds.Except(forecastedMatchIds).ToList();

        foreach (var matchId in missingMatchIds)
        {
            await GenerateForecastsForMatchAsync(matchId);
        }

        return Result<bool>.Success(true, $"Forecasts generated for {missingMatchIds.Count} missing matches.");
    }

    public async Task<Result<bool>> UpdateForecastsAfterMatchResultAsync(int matchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _matchReadRepository.GetAll()
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == matchId);

            if (match == null || !match.IsCompleted)
            {
                return Result<bool>.Failure("Valid completed match not found", 404);
            }

            var forecasts = await _readRepository.GetAll()
                .Where(x => x.MatchId == matchId)
                .ToListAsync();

            foreach (var forecast in forecasts)
            {
                forecast.IsCorrect = match.Outcomes.Any(o => o.TeamId == forecast.TeamId && o.OutcomeId == forecast.OutcomeId);

                var aiForecastResult = await _aiForecastService.GetByForecastIdAsync(forecast.Id);
                if (aiForecastResult.IsSuccess && aiForecastResult.Value is { } ai)
                {
                    bool isCorrectAi = ForecastHelper.CheckIsCorrect(match, ai.PredictedOutcomeName);

                    await _aiForecastService.SetIsCorrectAsync(forecast.Id, isCorrectAi);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, "Forecasts updated with correctness information.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Result<List<ForecastDto>>> GetByMatchIdAsync(int matchId)
    {
        var forecasts = await _readRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .ToListAsync();

        var dtoList = _mapper.Map<List<ForecastDto>>(forecasts);
        return Result<List<ForecastDto>>.Success(dtoList);
    }

    public async Task<Result<PagedResult<ForecastDto>>> GetAllForecastsAsync(ForecastFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAllWithIncludes();

        if (filters.LeagueId.HasValue)
        {
            query = query.Where(x => x.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));
        }

        if (filters.TeamId.HasValue)
        {
            query = query.Where(x => x.TeamId == filters.TeamId.Value);
        }

        if (filters.OutcomeId.HasValue)
        {
            query = query.Where(x => x.OutcomeId == filters.OutcomeId.Value);
        }

        if (filters.IsCorrect.HasValue)
        {
            query = query.Where(x => x.IsCorrect == filters.IsCorrect.Value);
        }

        var projected = query
            .OrderByDescending(x => x.Match.MatchDate)
            .ProjectTo<ForecastDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<ForecastDto>.CreateAsync(projected, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result<PagedResult<ForecastDto>>.Success(pagedResult);
    }




    public async Task<Result<List<ForecastSummaryDto>>> GetAllForecastSummariesAsync(ForecastFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Team)
            .Include(x => x.Match)
                .ThenInclude(m => m.Team1)
            .Include(x => x.Match)
                .ThenInclude(m => m.Team2)
            .Include(x => x.Match)
                .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Include(x => x.Outcome)
            .Where(x => !x.Match.IsCompleted);

        if (filters.LeagueId.HasValue)
        {
            query = query.Where(x => x.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));
        }

        var forecasts = await query.ToListAsync(cancellationToken);

        var summaries = new List<ForecastSummaryDto>();
        var grouped = forecasts.GroupBy(f => f.MatchId);

        foreach (var g in grouped)
        {
            var match = g.First().Match;
            var team1 = match.Team1?.Name ?? "Team 1";
            var team2 = match.Team2?.Name ?? "Team 2";

            var outcomeGroups = g.GroupBy(f => f.Outcome.Name)
                .Select(og => new OutcomeConfidenceDto
                {
                    OutcomeName = og.Key,
                    ConfidenceRatio = Math.Round(og.Average(f => f.Ratio), 3)
                }).ToList();

            var best = outcomeGroups.OrderByDescending(x => x.ConfidenceRatio).FirstOrDefault();
            var aiPrediction = await _forecastAiService.PredictBestOutcomeAsync(g.Key);

            summaries.Add(new ForecastSummaryDto
            {
                MatchId = g.Key,
                Team1Name = team1,
                Team2Name = team2,
                OutcomeForecasts = outcomeGroups,
                BestForecastOutcomeName = best?.OutcomeName ?? "N/A",
                BestConfidenceRatio = best?.ConfidenceRatio ?? 0,
                BestForecastOutcomeNameByAI = aiPrediction.OutcomeName,
                BestConfidenceRatioByAI = aiPrediction.Confidence
            });
        }

        return Result<List<ForecastSummaryDto>>.Success(summaries);
    }

    public async Task<Result<ForecastSummaryDto>> GetForecastSummaryByMatchIdAsync(int matchId)
    {
        var forecasts = await _readRepository.GetAll()
            .Where(x => x.MatchId == matchId)
            .Include(x => x.Team)
            .Include(x => x.Match)
                .ThenInclude(m => m.Team1)
            .Include(x => x.Match)
                .ThenInclude(m => m.Team2)
            .Include(x => x.Outcome)
            .ToListAsync();

        if (!forecasts.Any())
            return Result<ForecastSummaryDto>.Failure("Forecasts not found for this match", 404);

        var match = forecasts.First().Match;
        var team1 = match.Team1?.Name ?? "Team 1";
        var team2 = match.Team2?.Name ?? "Team 2";

        var outcomeGroups = forecasts.GroupBy(f => f.Outcome.Name)
            .Select(g => new OutcomeConfidenceDto
            {
                OutcomeName = g.Key,
                ConfidenceRatio = Math.Round(g.Average(x => x.Ratio), 3)
            }).ToList();

        var best = outcomeGroups.OrderByDescending(x => x.ConfidenceRatio).FirstOrDefault();
        var aiPrediction = await _forecastAiService.PredictBestOutcomeAsync(matchId);

        var summary = new ForecastSummaryDto
        {
            MatchId = matchId,
            Team1Name = team1,
            Team2Name = team2,
            OutcomeForecasts = outcomeGroups,
            BestForecastOutcomeName = best?.OutcomeName ?? "N/A",
            BestConfidenceRatio = best?.ConfidenceRatio ?? 0,
            BestForecastOutcomeNameByAI = aiPrediction.OutcomeName,
            BestConfidenceRatioByAI = aiPrediction.Confidence
        };

        return Result<ForecastSummaryDto>.Success(summary);
    }
    
    public async Task<Result<double>> GetClassicForecastAccuracyAsync(ForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsCorrect.HasValue);

        if (filters.LeagueId.HasValue)
        {
            query = query.Where(x => x.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));
        }

        if (filters.TeamId.HasValue)
        {
            query = query.Where(x => x.TeamId == filters.TeamId.Value);
        }

        if (filters.OutcomeId.HasValue)
        {
            query = query.Where(x => x.OutcomeId == filters.OutcomeId.Value);
        }

        if (filters.IsCorrect.HasValue)
        {
            query = query.Where(x => x.IsCorrect == filters.IsCorrect.Value);
        }

        var forecasts = await query.ToListAsync();

        if (forecasts.Count == 0)
        {
            return Result<double>.Success(0);
        }

        int correctCount = forecasts.Count(x => x.IsCorrect == true);
        double accuracy = (double)correctCount / forecasts.Count * 100;

        return Result<double>.Success(Math.Round(accuracy, 2));
    }
    
    public async Task<Result<PagedResult<ForecastTrendDto>>> GetTrendReportAsync(ForecastFilterParams filters)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues)
            .Where(x => x.IsCorrect.HasValue);

        if (filters.StartDate.HasValue)
        {
            query = query.Where(x => x.Match.MatchDate >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(x => x.Match.MatchDate <= filters.EndDate.Value);
        }

        if (filters.LeagueId.HasValue)
        {
            query = query.Where(x => x.Match.MatchTeamSeasonLeagues
                .Any(mtsl => mtsl.LeagueId == filters.LeagueId.Value));
        }

        var groupedQuery = query
            .GroupBy(x => x.Match.MatchDate.Date)
            .Select(g => new ForecastTrendDto
            {
                Date = g.Key,
                Total = g.Count(),
                Correct = g.Count(x => x.IsCorrect == true),
                Accuracy = Math.Round((double)g.Count(x => x.IsCorrect == true) / g.Count() * 100, 2)
            })
            .OrderByDescending(x => x.Date);

        var pagedResult = await PagedResult<ForecastTrendDto>.CreateAsync(
            groupedQuery,
            filters.PageNumber,
            filters.PageSize,
            CancellationToken.None);

        return Result<PagedResult<ForecastTrendDto>>.Success(pagedResult);
    }


}
