using Application.Core;
using Application.DTOs.Teams;
using Application.Params;
using Application.Repositories;
using Application.Repositories.TeamRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class TeamService : ITeamService
{
    private readonly ITeamReadRepository _readRepository;
    private readonly ITeamWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeamService(
        ITeamReadRepository readRepository,
        ITeamWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TeamDto>> CreateAsync(CreateTeamDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(x => x.Name.ToLower() == dto.Name.ToLower());
            if (duplicate is not null)
                return Result<TeamDto>.Failure(MessageGenerator.AlreadyExists("Team"), 409);

            var team = _mapper.Map<Team>(dto);

            await _writeRepository.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            team.TeamSeasonLeagues = BuildTeamSeasonLeagues(team.Id, dto.SeasonId, dto.LeagueId);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<TeamDto>(team);
            return Result<TeamDto>.Success(resultDto, MessageGenerator.CreationSuccess("Team"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateTeamDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var team = await _readRepository.GetAll()
                .Include(x => x.TeamSeasonLeagues)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (team == null || team.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Team"), 404);

            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Id != dto.Id && x.Name.ToLower() == dto.Name.ToLower());

            if (duplicate is not null)
                return Result<bool>.Failure(MessageGenerator.DuplicateExists("Team"), 409);

            _writeRepository.RemoveTeamSeasonLeagues(team);
            _mapper.Map(dto, team);

            team.TeamSeasonLeagues = BuildTeamSeasonLeagues(team.Id, dto.SeasonId, dto.LeagueId);

            _writeRepository.Update(team);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Team"));
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
            var team = await _readRepository.GetAll()
                .Include(x => x.TeamSeasonLeagues)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (team == null || team.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Team"), 404);

            _writeRepository.RemoveTeamSeasonLeagues(team);

            team.IsDeleted = true;
            _writeRepository.Update(team);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Team"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<TeamDto>> GetByIdAsync(int id)
    {
        var team = await _readRepository.GetAll()
            .Include(x => x.TeamSeasonLeagues)
                .ThenInclude(x => x.League)
            .Include(x => x.Country)
            .Include(x => x.TeamSeasonLeagues)
                .ThenInclude(x => x.Season)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (team == null || team.IsDeleted)
            return Result<TeamDto>.Failure(MessageGenerator.NotFound("Team"), 404);

        var dto = _mapper.Map<TeamDto>(team);
        return Result<TeamDto>.Success(dto);
    }

    public async Task<Result<PagedResult<TeamDto>>> GetAllAsync(TeamFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        if (filters.CountryId.HasValue)
            query = query.Where(x => x.CountryId == filters.CountryId.Value);

        if (filters.SeasonId.HasValue)
            query = query.Where(x => x.TeamSeasonLeagues.Any(tsl => tsl.SeasonId == filters.SeasonId.Value));

        if (filters.LeagueId.HasValue)
            query = query.Where(x => x.TeamSeasonLeagues.Any(tsl => tsl.LeagueId == filters.LeagueId.Value));

        query = query.OrderBy(x => x.Name);

        var projected = query.ProjectTo<TeamDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<TeamDto>.CreateAsync(
            projected,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<TeamDto>>.Success(pagedResult);
    }

    private static List<TeamSeasonLeague> BuildTeamSeasonLeagues(int teamId, int seasonId, int leagueId)
    {
        return new List<TeamSeasonLeague>
        {
            new TeamSeasonLeague
            {
                TeamId = teamId,
                SeasonId = seasonId,
                LeagueId = leagueId
            }
        };
    }
}