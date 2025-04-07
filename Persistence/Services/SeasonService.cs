using Application.Core;
using Application.DTOs.Seasons;
using Application.Params;
using Application.Repositories;
using Application.Repositories.SeasonRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class SeasonService : ISeasonService
{
    private readonly ISeasonReadRepository _readRepository;
    private readonly ISeasonWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SeasonService(
        ISeasonReadRepository readRepository,
        ISeasonWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SeasonDto>> GetByIdAsync(string id)
    {
        var season = await _readRepository.GetByIdAsync(id);
        if (season == null || season.IsDeleted)
            return Result<SeasonDto>.Failure(MessageGenerator.NotFound("Season"), 404);

        var dto = _mapper.Map<SeasonDto>(season);
        return Result<SeasonDto>.Success(dto);
    }

    public async Task<Result<PagedResult<SeasonDto>>> GetAllAsync(SeasonFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        if (filters.StartDateFrom.HasValue)
            query = query.Where(x => x.StartDate >= filters.StartDateFrom.Value);

        if (filters.StartDateTo.HasValue)
            query = query.Where(x => x.StartDate <= filters.StartDateTo.Value);

        if (filters.EndDateFrom.HasValue)
            query = query.Where(x => x.EndDate >= filters.EndDateFrom.Value);

        if (filters.EndDateTo.HasValue)
            query = query.Where(x => x.EndDate <= filters.EndDateTo.Value);

        query = query.OrderBy(x => x.StartDate);

        var projectedQuery = query.ProjectTo<SeasonDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<SeasonDto>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<SeasonDto>>.Success(pagedResult);
    }

    public async Task<Result<SeasonDto>> CreateAsync(CreateSeasonDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Name.ToLower() == dto.Name.ToLower() &&
                x.StartDate == dto.StartDate &&
                x.EndDate == dto.EndDate);

            if (duplicate is not null)
                return Result<SeasonDto>.Failure(MessageGenerator.AlreadyExists("Season"), 409);

            var season = _mapper.Map<Season>(dto);
            await _writeRepository.AddAsync(season);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<SeasonDto>(season);
            return Result<SeasonDto>.Success(resultDto, MessageGenerator.CreationSuccess("Season"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateSeasonDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var season = await _readRepository.GetByIdAsync(dto.Id);
            if (season == null || season.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Season"), 404);

            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Id != dto.Id &&
                x.Name.ToLower() == dto.Name.ToLower() &&
                x.StartDate == dto.StartDate &&
                x.EndDate == dto.EndDate);

            if (duplicate is not null)
                return Result<bool>.Failure(MessageGenerator.DuplicateExists("Season"), 409);

            _mapper.Map(dto, season);
            _writeRepository.Update(season);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Season"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var season = await _readRepository.GetByIdAsync(id);
            if (season == null || season.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Season"), 404);

            season.IsDeleted = true;
            _writeRepository.Update(season);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Season"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
