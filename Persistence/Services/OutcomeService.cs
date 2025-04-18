using Application.Core;
using Application.DTOs.Outcomes;
using Application.Params;
using Application.Repositories;
using Application.Repositories.OutcomeRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class OutcomeService : IOutcomeService
{
    private readonly IOutcomeReadRepository _readRepository;
    private readonly IOutcomeWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OutcomeService(
        IOutcomeReadRepository readRepository,
        IOutcomeWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OutcomeDto>> GetByIdAsync(int id)
    {
        var outcome = await _readRepository.GetByIdAsync(id);
        if (outcome == null || outcome.IsDeleted)
            return Result<OutcomeDto>.Failure(MessageGenerator.NotFound("Outcome"), 404);

        var dto = _mapper.Map<OutcomeDto>(outcome);
        return Result<OutcomeDto>.Success(dto);
    }

    public async Task<Result<PagedResult<OutcomeDto>>> GetAllAsync(OutcomeFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        query = query.OrderBy(x => x.Name);

        var projectedQuery = query.ProjectTo<OutcomeDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<OutcomeDto>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<OutcomeDto>>.Success(pagedResult);
    }

    public async Task<Result<OutcomeDto>> CreateAsync(CreateOutcomeDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(x => x.Name.ToLower() == dto.Name.ToLower());
            if (duplicate is not null)
                return Result<OutcomeDto>.Failure(MessageGenerator.AlreadyExists("Outcome"), 409);

            var outcome = _mapper.Map<Outcome>(dto);
            await _writeRepository.AddAsync(outcome);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<OutcomeDto>(outcome);
            return Result<OutcomeDto>.Success(resultDto, MessageGenerator.CreationSuccess("Outcome"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateOutcomeDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var outcome = await _readRepository.GetByIdAsync(dto.Id);
            if (outcome == null || outcome.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Outcome"), 404);

            var duplicate = await _readRepository.GetSingleAsync(x =>
                x.Id != dto.Id && x.Name.ToLower() == dto.Name.ToLower());

            if (duplicate is not null)
                return Result<bool>.Failure(MessageGenerator.DuplicateExists("Outcome"), 409);

            _mapper.Map(dto, outcome);
            _writeRepository.Update(outcome);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Outcome"));
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
            var outcome = await _readRepository.GetByIdAsync(id);
            if (outcome == null || outcome.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Outcome"), 404);

            outcome.IsDeleted = true;
            _writeRepository.Update(outcome);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Outcome"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
