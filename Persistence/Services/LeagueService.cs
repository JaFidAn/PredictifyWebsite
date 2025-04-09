using Application.Core;
using Application.DTOs.Leagues;
using Application.Params;
using Application.Repositories;
using Application.Repositories.CompetitionRepositories;
using Application.Repositories.LeagueRepositories;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeagueService(
        ILeagueReadRepository readRepository,
        ILeagueWriteRepository writeRepository,
        ICompetitionWriteRepository competitionWriteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _competitionWriteRepository = competitionWriteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<LeagueDto>> GetByIdAsync(int id)
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

        if (filters.CountryId.HasValue)
            query = query.Where(x => x.CountryId == filters.CountryId.Value);

        if (filters.CompetitionId.HasValue)
            query = query.Where(x => x.CompetitionId == filters.CompetitionId.Value);

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
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(
                x => x.Name.ToLower() == dto.Name.ToLower() && x.CountryId == dto.CountryId);

            if (duplicate is not null)
                return Result<LeagueDto>.Failure(MessageGenerator.AlreadyExists("League"), 409);

            var league = _mapper.Map<League>(dto);

            if (dto.CompetitionId.HasValue)
            {
                league.CompetitionId = dto.CompetitionId.Value;
            }
            else
            {
                var competition = new Competition
                {
                    Name = dto.Name,
                    IsInternational = false
                };

                await _competitionWriteRepository.AddAsync(competition);
                await _unitOfWork.SaveChangesAsync();

                league.CompetitionId = competition.Id;
            }

            await _writeRepository.AddAsync(league);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<LeagueDto>(league);
            return Result<LeagueDto>.Success(resultDto, MessageGenerator.CreationSuccess("League"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateLeagueDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
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

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("League"));
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
            var league = await _readRepository.GetByIdAsync(id);
            if (league == null || league.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("League"), 404);

            league.IsDeleted = true;
            _writeRepository.Update(league);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("League"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
