using Application.Core;
using Application.DTOs.Matches;
using Application.Params;
using Application.Repositories;
using Application.Repositories.MatchRepositories;
using Application.Repositories.OutcomeRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Helpers;

namespace Persistence.Services;

public class MatchService : IMatchService
{
    private readonly IMatchReadRepository _readRepository;
    private readonly IMatchWriteRepository _writeRepository;
    private readonly IOutcomeReadRepository _outcomeReadRepository;
    private readonly ITeamOutcomeStreakService _streakService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MatchService(
        IMatchReadRepository readRepository,
        IMatchWriteRepository writeRepository,
        IOutcomeReadRepository outcomeReadRepository,
        ITeamOutcomeStreakService streakService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _outcomeReadRepository = outcomeReadRepository;
        _streakService = streakService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MatchDto>> CreateAsync(CreateMatchDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (dto.Team1Id == dto.Team2Id)
                return Result<MatchDto>.Failure("A team cannot play against itself", 400);

            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Team1Id == dto.Team1Id && x.Team2Id == dto.Team2Id && x.MatchDate == dto.MatchDate);

            if (duplicate is not null)
                return Result<MatchDto>.Failure(MessageGenerator.AlreadyExists("Match"), 409);

            var match = _mapper.Map<Match>(dto);
            match.IsCompleted = dto.Team1Goals.HasValue && dto.Team2Goals.HasValue;

            await _writeRepository.AddAsync(match);
            await _unitOfWork.SaveChangesAsync();

            match.MatchTeamSeasonLeagues = BuildMatchTeamSeasonLeagues(
                match.Id, dto.Team1Id, dto.Team2Id, dto.SeasonId, dto.LeagueId);

            var outcomes = await _outcomeReadRepository.GetAll().ToListAsync();
            match.Outcomes = OutcomeDeterminationHelper.DetermineOutcomes(match, outcomes);

            await _unitOfWork.SaveChangesAsync();

            if (match.IsCompleted)
            {
                await _streakService.RecalculateStreaksForMatchAsync(match.Id);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitAsync();

            var resultDto = await GetByIdAsync(match.Id);
            return resultDto;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateMatchDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _readRepository.GetAll()
                .Include(x => x.MatchTeamSeasonLeagues)
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (match == null || match.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Match"), 404);

            match.MatchDate = dto.MatchDate;
            match.Team1Goals = dto.Team1Goals;
            match.Team2Goals = dto.Team2Goals;
            match.IsCompleted = dto.Team1Goals.HasValue && dto.Team2Goals.HasValue;

            _writeRepository.RemoveMatchTeamSeasonLeagues(match);
            _writeRepository.RemoveMatchOutcomes(match);

            match.MatchTeamSeasonLeagues = BuildMatchTeamSeasonLeagues(
                match.Id, dto.Team1Id, dto.Team2Id, dto.SeasonId, dto.LeagueId);

            var outcomes = await _outcomeReadRepository.GetAll().ToListAsync();
            match.Outcomes = OutcomeDeterminationHelper.DetermineOutcomes(match, outcomes);

            _writeRepository.Update(match);
            await _unitOfWork.SaveChangesAsync();

            if (match.IsCompleted)
            {
                await _streakService.RecalculateStreaksForMatchAsync(match.Id);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Match"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var match = await _readRepository.GetAll()
                .Include(x => x.MatchTeamSeasonLeagues)
                .Include(x => x.Outcomes)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (match == null || match.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Match"), 404);

            _writeRepository.RemoveMatchTeamSeasonLeagues(match);
            _writeRepository.RemoveMatchOutcomes(match);

            match.IsDeleted = true;
            _writeRepository.Update(match);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Match"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<MatchDto>> GetByIdAsync(int id)
    {
        var match = await _readRepository.GetAll()
            .Include(x => x.Team1)
            .Include(x => x.Team2)
            .Include(x => x.MatchTeamSeasonLeagues).ThenInclude(x => x.Season)
            .Include(x => x.MatchTeamSeasonLeagues).ThenInclude(x => x.League)
            .Include(x => x.Outcomes).ThenInclude(x => x.Outcome)
            .Include(x => x.Outcomes).ThenInclude(x => x.Team)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (match == null || match.IsDeleted)
            return Result<MatchDto>.Failure(MessageGenerator.NotFound("Match"), 404);

        var dto = _mapper.Map<MatchDto>(match);
        return Result<MatchDto>.Success(dto);
    }

    public async Task<Result<PagedResult<MatchDto>>> GetAllAsync(MatchFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (filters.TeamId.HasValue)
            query = query.Where(x => x.Team1Id == filters.TeamId || x.Team2Id == filters.TeamId);

        query = query.OrderByDescending(x => x.MatchDate);

        var projected = query.ProjectTo<MatchDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<MatchDto>.CreateAsync(
            projected,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<MatchDto>>.Success(pagedResult);
    }

    private static List<MatchTeamSeasonLeague> BuildMatchTeamSeasonLeagues(int matchId, int team1Id, int team2Id, int seasonId, int leagueId)
    {
        return new List<MatchTeamSeasonLeague>
        {
            new MatchTeamSeasonLeague
            {
                MatchId = matchId,
                TeamId = team1Id,
                SeasonId = seasonId,
                LeagueId = leagueId
            },
            new MatchTeamSeasonLeague
            {
                MatchId = matchId,
                TeamId = team2Id,
                SeasonId = seasonId,
                LeagueId = leagueId
            }
        };
    }
}
