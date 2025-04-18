using Application.Core;
using Application.DTOs.Competitions;
using Application.Params;
using Application.Repositories;
using Application.Repositories.CompetitionRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class CompetitionService : ICompetitionService
{
    private readonly ICompetitionReadRepository _readRepository;
    private readonly ICompetitionWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompetitionService(
        ICompetitionReadRepository readRepository,
        ICompetitionWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CompetitionDto>> GetByIdAsync(int id)
    {
        var competition = await _readRepository.GetByIdAsync(id);
        if (competition == null || competition.IsDeleted)
            return Result<CompetitionDto>.Failure(MessageGenerator.NotFound("Competition"), 404);

        var dto = _mapper.Map<CompetitionDto>(competition);
        return Result<CompetitionDto>.Success(dto);
    }

    public async Task<Result<PagedResult<CompetitionDto>>> GetAllAsync(CompetitionFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        if (filters.IsInternational is not null)
            query = query.Where(x => x.IsInternational == filters.IsInternational);

        query = query.OrderBy(x => x.Name);

        var projectedQuery = query.ProjectTo<CompetitionDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<CompetitionDto>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<CompetitionDto>>.Success(pagedResult);
    }

    public async Task<Result<CompetitionDto>> CreateAsync(CreateCompetitionDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(x => x.Name.ToLower() == dto.Name.ToLower());
            if (duplicate is not null)
                return Result<CompetitionDto>.Failure(MessageGenerator.AlreadyExists("Competition"), 409);

            var competition = _mapper.Map<Competition>(dto);

            await _writeRepository.AddAsync(competition);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<CompetitionDto>(competition);
            return Result<CompetitionDto>.Success(resultDto, MessageGenerator.CreationSuccess("Competition"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateCompetitionDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var competition = await _readRepository.GetByIdAsync(dto.Id);
            if (competition == null || competition.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Competition"), 404);

            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Id != dto.Id && x.Name.ToLower() == dto.Name.ToLower());

            if (duplicate is not null)
                return Result<bool>.Failure(MessageGenerator.DuplicateExists("Competition"), 409);

            _mapper.Map(dto, competition);
            _writeRepository.Update(competition);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Competition"));
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
            var competition = await _readRepository.GetByIdAsync(id);
            if (competition == null || competition.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Competition"), 404);

            competition.IsDeleted = true;
            _writeRepository.Update(competition);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Competition"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
