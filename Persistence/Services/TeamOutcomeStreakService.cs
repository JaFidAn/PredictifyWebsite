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

    public async Task<Result<TeamOutcomeStreakDto>> GetByIdAsync(int id)
    {
        var streak = await _readRepository.GetAll()
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .FirstOrDefaultAsync(x => x.Id == id);

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
                await RecalculateStreaksForTeamMatchAsync(teamId, match);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, "Streaks calculated based on actual outcomes.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public Task<Result<bool>> RecalculateAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RecalculateStreaksForTeamAsync(int teamId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RecalculateStreaksUpToMatchAsync(int teamId, int matchId)
    {
        throw new NotImplementedException();
    }

    private async Task RecalculateStreaksForTeamMatchAsync(int teamId, Match match)
    {
        var allOutcomes = await _outcomeReadRepository.GetAll().ToListAsync();

        // Real nəticəyə əsaslanan outcome-ları al
        var applicableOutcomeIds = OutcomeDeterminationHelper
            .DetermineOutcomesForTeam(match, allOutcomes, teamId)
            .ToHashSet();

        foreach (var outcome in allOutcomes)
        {
            var lastStreak = await _readRepository.GetAll()
                .Where(x => x.TeamId == teamId && x.OutcomeId == outcome.Id && x.MatchDate < match.MatchDate)
                .OrderByDescending(x => x.MatchDate)
                .FirstOrDefaultAsync();

            bool occurred = applicableOutcomeIds.Contains(outcome.Id);

            int newStreakCount = occurred ? 0 : (lastStreak?.StreakCount + 1 ?? 1);

            var newStreak = new TeamOutcomeStreak
            {
                TeamId = teamId,
                OutcomeId = outcome.Id,
                StreakCount = newStreakCount,
                MatchId = match.Id,
                MatchDate = match.MatchDate
            };

            await _writeRepository.AddAsync(newStreak);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
