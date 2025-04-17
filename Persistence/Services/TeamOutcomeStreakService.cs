using Application.Core;
using Application.DTOs.TeamOutcomeStreaks;
using Application.Params;
using Application.Repositories;
using Application.Repositories.MatchRepositories;
using Application.Repositories.OutcomeRepositories;
using Application.Repositories.TeamOutcomeStreakRepositories;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Helpers;

namespace Persistence.Services;

public class TeamOutcomeStreakService : ITeamOutcomeStreakService
{
    private readonly IMatchReadRepository _matchReadRepository;
    private readonly IOutcomeReadRepository _outcomeReadRepository;
    private readonly ITeamOutcomeStreakReadRepository _readRepository;
    private readonly ITeamOutcomeStreakWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeamOutcomeStreakService(
        IMatchReadRepository matchReadRepository,
        IOutcomeReadRepository outcomeReadRepository,
        ITeamOutcomeStreakReadRepository readRepository,
        ITeamOutcomeStreakWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _matchReadRepository = matchReadRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<TeamOutcomeStreakDto>>> GetAllAsync(TeamOutcomeStreakFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll()
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .AsQueryable();

        if (filters.TeamId.HasValue)
        {
            query = query.Where(x => x.TeamId == filters.TeamId);
        }

        if (filters.OutcomeId.HasValue)
        {
            query = query.Where(x => x.OutcomeId == filters.OutcomeId);
        }

        query = query.OrderByDescending(x => x.MatchDate);

        var projected = query.ProjectTo<TeamOutcomeStreakDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<TeamOutcomeStreakDto>.CreateAsync(
            projected,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TeamOutcomeStreakDto>>.Success(pagedResult);
    }

    public async Task<Result<TeamOutcomeStreakDto>> GetByIdsAsync(int teamId, int outcomeId, int matchId)
    {
        var streak = await _readRepository.GetAll()
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .FirstOrDefaultAsync(x =>
                x.TeamId == teamId &&
                x.OutcomeId == outcomeId &&
                x.MatchId == matchId);

        if (streak == null)
        {
            return Result<TeamOutcomeStreakDto>.Failure("Streak not found", 404);
        }

        var dto = _mapper.Map<TeamOutcomeStreakDto>(streak);
        return Result<TeamOutcomeStreakDto>.Success(dto);
    }

    public async Task<Result<bool>> RecalculateStreaksForMatchAsync(int matchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _matchReadRepository.GetAll()
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == matchId);

            if (match == null || !match.IsCompleted || match.IsDeleted)
            {
                return Result<bool>.Failure("Valid and completed match not found", 404);
            }

            var teamIds = match.Outcomes.Select(x => x.TeamId).Distinct().ToList();

            foreach (var teamId in teamIds)
            {
                await RecalculateStreaksUpToMatchAsync(teamId, match.Id);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, "Streaks recalculated based on actual outcomes.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> RecalculateAllAsync()
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var matches = await _matchReadRepository.GetAll()
                .Include(x => x.Outcomes) 
                .Where(x => x.IsCompleted && !x.IsDeleted)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            var teamIds = matches.SelectMany(m => m.Outcomes.Select(o => o.TeamId)).Distinct();

            foreach (var teamId in teamIds)
            {
                await RecalculateStreaksUpToMatchAsync(teamId, int.MaxValue);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, "All streaks recalculated.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> RecalculateStreaksForTeamAsync(int teamId)
    {
        return await RecalculateStreaksUpToMatchAsync(teamId, int.MaxValue);
    }

    public async Task<Result<bool>> RecalculateStreaksUpToMatchAsync(int teamId, int matchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            DateTime maxDate;

            if (matchId == int.MaxValue)
            {
                maxDate = DateTime.MaxValue;
            }
            else
            {
                var targetMatch = await _matchReadRepository.GetByIdAsync(matchId);
                if (targetMatch == null)
                {
                    return Result<bool>.Failure("Match not found", 404);
                }

                maxDate = targetMatch.MatchDate;
            }

            var matches = await _matchReadRepository.GetAll()
                .Include(x => x.Outcomes) 
                .Where(x => x.IsCompleted && !x.IsDeleted && x.Outcomes.Any(o => o.TeamId == teamId) && x.MatchDate <= maxDate)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            await _writeRepository.RemoveRangeByTeamAndMatchDateAsync(teamId, maxDate);

            var allOutcomes = await _outcomeReadRepository.GetAll().ToListAsync();
            var streakTracker = new Dictionary<int, int>();
            var maxTracker = new Dictionary<int, int>();

            foreach (var match in matches)
            {
                var applicableOutcomeIds = OutcomeDeterminationHelper
                    .DetermineOutcomesForTeam(match, allOutcomes, teamId)
                    .ToHashSet();

                foreach (var outcome in allOutcomes)
                {
                    var outcomeId = outcome.Id;
                    var occurred = applicableOutcomeIds.Contains(outcomeId);

                    streakTracker.TryGetValue(outcomeId, out int prevStreak);
                    int newStreak = occurred ? 0 : prevStreak + 1;
                    streakTracker[outcomeId] = newStreak;

                    maxTracker.TryGetValue(outcomeId, out int maxSoFar);
                    if (newStreak > maxSoFar)
                    {
                        maxSoFar = newStreak;
                        maxTracker[outcomeId] = newStreak;
                    }

                    double ratio = maxSoFar > 0 ? Math.Round((double)newStreak / maxSoFar, 3) : 0;

                    await _writeRepository.AddAsync(new TeamOutcomeStreak
                    {
                        TeamId = teamId,
                        OutcomeId = outcomeId,
                        MatchId = match.Id,
                        MatchDate = match.MatchDate,
                        StreakCount = newStreak,
                        MaxStreak = maxSoFar,
                        Ratio = ratio
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, $"Streaks recalculated for team {teamId} up to match {matchId}.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<List<TeamOutcomeStreakDto>>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken)
    {
        var streaks = await _readRepository.GetByTeamIdAsync(teamId, cancellationToken);

        if (streaks.Count == 0)
        {
            return Result<List<TeamOutcomeStreakDto>>.Failure("No streaks found for this team", 404);
        }

        var dtoList = _mapper.Map<List<TeamOutcomeStreakDto>>(streaks);
        return Result<List<TeamOutcomeStreakDto>>.Success(dtoList);
    }

    public async Task<Result<List<TeamOutcomeStreakDto>>> GetLatestByTeamIdAsync(int teamId, CancellationToken cancellationToken)
    {
        var allStreaks = await _readRepository.GetAll()
            .Where(x => x.TeamId == teamId)
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .ToListAsync(cancellationToken);

        var latestStreaks = allStreaks
            .GroupBy(x => x.OutcomeId)
            .Select(g => g.OrderByDescending(x => x.MatchDate).First())
            .ToList();

        if (latestStreaks.Count == 0)
        {
            return Result<List<TeamOutcomeStreakDto>>.Failure("No streaks found for this team", 404);
        }

        var dtoList = _mapper.Map<List<TeamOutcomeStreakDto>>(latestStreaks);
        return Result<List<TeamOutcomeStreakDto>>.Success(dtoList);
    }

}
