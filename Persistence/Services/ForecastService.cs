using Application.Core;
using Application.DTOs.Forecasts;
using Application.DTOs.Outcomes;
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
    private readonly IMapper _mapper;

    public ForecastService(
        IMatchReadRepository matchReadRepository,
        IForecastWriteRepository writeRepository,
        IForecastReadRepository readRepository,
        ITeamOutcomeStreakReadRepository streakReadRepository,
        IOutcomeReadRepository outcomeReadRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _matchReadRepository = matchReadRepository;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _streakReadRepository = streakReadRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<bool>> GenerateForecastsForMatchAsync(int matchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _matchReadRepository.GetAll()
                .Include(x => x.MatchTeamSeasonLeagues)
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == matchId);

            if (match == null || match.IsDeleted)
                return Result<bool>.Failure("Match not found", 404);

            var existingForecasts = await _readRepository.GetAll()
                .Where(x => x.MatchId == matchId)
                .ToListAsync();

            if (existingForecasts.Any())
            {
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
        var matches = await _matchReadRepository.GetAll()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.MatchDate)
            .ToListAsync();

        foreach (var match in matches)
        {
            await GenerateForecastsForMatchAsync(match.Id);
        }

        return Result<bool>.Success(true, "Forecasts generated for all matches.");
    }

    public async Task<Result<bool>> UpdateForecastsAfterMatchResultAsync(int matchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _matchReadRepository.GetAll()
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == matchId);

            if (match == null || !match.IsCompleted || match.IsDeleted)
                return Result<bool>.Failure("Valid completed match not found", 404);

            var actualOutcomePairs = match.Outcomes
                .Select(x => new { x.TeamId, x.OutcomeId })
                .ToHashSet();

            var forecasts = await _readRepository.GetAll()
                .Where(x => x.MatchId == matchId && !x.IsDeleted)
                .ToListAsync();

            foreach (var forecast in forecasts)
            {
                forecast.IsCorrect = actualOutcomePairs.Contains(new { forecast.TeamId, forecast.OutcomeId });
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
            .Where(x => x.MatchId == matchId && !x.IsDeleted)
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .ToListAsync();

        var dtoList = _mapper.Map<List<ForecastDto>>(forecasts);
        return Result<List<ForecastDto>>.Success(dtoList);
    }

    public async Task<Result<List<ForecastSummaryDto>>> GetAllForecastSummariesAsync(CancellationToken cancellationToken)
    {
        var forecasts = await _readRepository.GetAll()
            .Where(x => !x.IsDeleted && x.Match.IsCompleted == false)
            .Include(x => x.Team)
            .Include(x => x.Match)
            .ThenInclude(m => m.Team1)
            .Include(x => x.Match)
            .ThenInclude(m => m.Team2)
            .Include(x => x.Outcome)
            .ToListAsync();

        var summaries = forecasts
            .GroupBy(f => f.MatchId)
            .Select(g =>
            {
                var match = g.First().Match;
                var team1 = match.Team1?.Name ?? "Team 1";
                var team2 = match.Team2?.Name ?? "Team 2";

                var outcomeGroups = g
                    .GroupBy(f => f.Outcome.Name)
                    .Select(og =>
                    {
                        double confidenceRatio = Math.Round(og.Average(f => f.Ratio), 3);
                        return new OutcomeConfidenceDto
                        {
                            OutcomeName = og.Key,
                            ConfidenceRatio = confidenceRatio
                        };
                    })
                    .ToList();

                var best = outcomeGroups.OrderByDescending(x => x.ConfidenceRatio).First();

                return new ForecastSummaryDto
                {
                    MatchId = g.Key,
                    Team1Name = team1,
                    Team2Name = team2,
                    OutcomeForecasts = outcomeGroups,
                    BestForecastOutcomeName = best.OutcomeName,
                    BestConfidenceRatio = best.ConfidenceRatio
                };
            })
            .ToList();

        return Result<List<ForecastSummaryDto>>.Success(summaries);
    }

    public async Task<Result<ForecastSummaryDto>> GetForecastSummaryByMatchIdAsync(int matchId)
    {
        var forecasts = await _readRepository.GetAll()
            .Where(x => !x.IsDeleted && x.MatchId == matchId)
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

        var outcomeGroups = forecasts
            .GroupBy(f => f.Outcome.Name)
            .Select(g =>
            {
                double confidenceRatio = Math.Round(g.Average(x => x.Ratio), 3);
                return new OutcomeConfidenceDto
                {
                    OutcomeName = g.Key,
                    ConfidenceRatio = confidenceRatio
                };
            })
            .ToList();

        var best = outcomeGroups.OrderByDescending(x => x.ConfidenceRatio).First();

        var summary = new ForecastSummaryDto
        {
            MatchId = matchId,
            Team1Name = team1,
            Team2Name = team2,
            OutcomeForecasts = outcomeGroups,
            BestForecastOutcomeName = best.OutcomeName,
            BestConfidenceRatio = best.ConfidenceRatio
        };

        return Result<ForecastSummaryDto>.Success(summary);
    }

    public async Task<Result<PagedResult<ForecastDto>>> GetAllForecastsAsync(ForecastFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
            .Where(x => !x.IsDeleted);

        if (filters.MatchId.HasValue)
        {
            query = query.Where(x => x.MatchId == filters.MatchId.Value);
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
    
    public async Task<Result<bool>> GenerateForecastsForMissingMatchesAsync()
    {
        var allMatchIds = await _matchReadRepository.GetAll()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync();

        var forecastedMatchIds = await _readRepository.GetAll()
            .Where(x => !x.IsDeleted)
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
}
