using Application.Core;
using Application.DTOs.Countries;
using Application.Params;
using Application.Repositories;
using Application.Repositories.CountryRepositories;
using Application.Services;
using Application.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Persistence.Services;

public class CountryService : ICountryService
{
    private readonly ICountryReadRepository _readRepository;
    private readonly ICountryWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CountryService(
        ICountryReadRepository readRepository,
        ICountryWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CountryDto>> GetByIdAsync(int id)
    {
        var country = await _readRepository.GetByIdAsync(id);
        if (country == null || country.IsDeleted)
            return Result<CountryDto>.Failure(MessageGenerator.NotFound("Country"), 404);

        var dto = _mapper.Map<CountryDto>(country);
        return Result<CountryDto>.Success(dto);
    }

    public async Task<Result<PagedResult<CountryDto>>> GetAllAsync(CountryFilterParams filters, CancellationToken cancellationToken)
    {
        var query = _readRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filters.Name))
            query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(filters.Code))
            query = query.Where(x => x.Code.ToLower().Contains(filters.Code.ToLower()));

        query = query.OrderBy(x => x.Name);

        var projectedQuery = query.ProjectTo<CountryDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<CountryDto>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
        );

        return Result<PagedResult<CountryDto>>.Success(pagedResult);
    }

    public async Task<Result<CountryDto>> CreateAsync(CreateCountryDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var duplicate = await _readRepository.GetSingleAsync(
                x => x.Name.ToLower() == dto.Name.ToLower() || x.Code.ToLower() == dto.Code.ToLower());

            if (duplicate is not null)
                return Result<CountryDto>.Failure(MessageGenerator.AlreadyExists("Country"), 409);

            var country = _mapper.Map<Country>(dto);

            await _writeRepository.AddAsync(country);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<CountryDto>(country);
            return Result<CountryDto>.Success(resultDto, MessageGenerator.CreationSuccess("Country"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdateCountryDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var country = await _readRepository.GetByIdAsync(dto.Id);
            if (country == null || country.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Country"), 404);

            var duplicate = await _readRepository.GetSingleAsync(
                x => x.Id != dto.Id &&
                     (x.Name.ToLower() == dto.Name.ToLower() || x.Code.ToLower() == dto.Code.ToLower()));

            if (duplicate is not null)
                return Result<bool>.Failure(MessageGenerator.DuplicateExists("Country"), 409);

            _mapper.Map(dto, country);
            _writeRepository.Update(country);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Country"));
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
            var country = await _readRepository.GetByIdAsync(id);
            if (country == null || country.IsDeleted)
                return Result<bool>.Failure(MessageGenerator.NotFound("Country"), 404);

            country.IsDeleted = true;
            _writeRepository.Update(country);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Country"));
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
