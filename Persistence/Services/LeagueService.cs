using Application.Core;
using Application.DTOs.Leagues;
using Application.Params;
using Application.Repositories.LeagueRepositories;
using Application.Repositories.CompetitionRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class LeagueService : ILeagueService
{
    private readonly ILeagueReadRepository _readRepository;
    private readonly ILeagueWriteRepository _writeRepository;
    private readonly ICompetitionWriteRepository _competitionWriteRepository;
    private readonly IMapper _mapper;

    public LeagueService(
        ILeagueReadRepository readRepository,
        ILeagueWriteRepository writeRepository,
        ICompetitionWriteRepository competitionWriteRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _competitionWriteRepository = competitionWriteRepository;
        _mapper = mapper;
    }

    public async Task<Result<LeagueDto>> GetByIdAsync(string id)
    {
        var league = await _readRepository.GetByIdAsync(id);
        if (league == null || league.IsDeleted)
            return Result<LeagueDto>.Failure(MessageGenerator.NotFound("League"), 404);

        var dto = _mapper.Map<LeagueDto>(league);
        return Result<LeagueDto>.Success(dto);
    }

    public async Task<Result<PagedResult<LeagueDto>>> GetAllAsync(LeagueFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.CountryId))
            query = query.Where(x => x.CountryId == filters.CountryId);

        if (!string.IsNullOrWhiteSpace(filters.CompetitionId))
            query = query.Where(x => x.CompetitionId == filters.CompetitionId);

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        query = query.OrderBy(x => x.Name);

        var projectedQuery = query.ProjectTo<LeagueDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<LeagueDto>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<LeagueDto>>.Success(pagedResult);
    }

    public async Task<Result<LeagueDto>> CreateAsync(CreateLeagueDto dto)
    {
        var duplicate = await _readRepository.GetSingleAsync(
            x => x.Name.ToLower() == dto.Name.ToLower() && x.CountryId == dto.CountryId);

        if (duplicate is not null)
            return Result<LeagueDto>.Failure(MessageGenerator.AlreadyExists("League"), 409);

        var league = _mapper.Map<League>(dto);

        if (!string.IsNullOrWhiteSpace(dto.CompetitionId))
        {
            league.CompetitionId = dto.CompetitionId;
        }
        else
        {
            var competition = new Competition
            {
                Name = dto.Name,
                IsInternational = false
            };

            await _competitionWriteRepository.AddAsync(competition);
            await _competitionWriteRepository.SaveAsync();

            league.CompetitionId = competition.Id;
        }

        await _writeRepository.AddAsync(league);
        await _writeRepository.SaveAsync();

        var resultDto = _mapper.Map<LeagueDto>(league);
        return Result<LeagueDto>.Success(resultDto, MessageGenerator.CreationSuccess("League"));
    }

    public async Task<Result<bool>> UpdateAsync(UpdateLeagueDto dto)
    {
        var league = await _readRepository.GetByIdAsync(dto.Id);
        if (league == null || league.IsDeleted)
            return Result<bool>.Failure(MessageGenerator.NotFound("League"), 404);

        var duplicate = await _readRepository.GetSingleAsync(
            x => x.Id != dto.Id &&
                 x.Name.ToLower() == dto.Name.ToLower() &&
                 x.CountryId == dto.CountryId);

        if (duplicate is not null)
            return Result<bool>.Failure(MessageGenerator.DuplicateExists("League"), 409);

        _mapper.Map(dto, league);
        _writeRepository.Update(league);
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("League"));
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var league = await _readRepository.GetByIdAsync(id);
        if (league == null || league.IsDeleted)
            return Result<bool>.Failure(MessageGenerator.NotFound("League"), 404);

        league.IsDeleted = true;
        _writeRepository.Update(league);
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("League"));
    }
}
