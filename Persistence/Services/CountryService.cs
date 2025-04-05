using Application.Core;
using Application.DTOs.Countries;
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
    private readonly IMapper _mapper;

    public CountryService(
        ICountryReadRepository readRepository,
        ICountryWriteRepository writeRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _mapper = mapper;
    }

    public async Task<Result<CountryDto>> GetByIdAsync(string id)
    {
        var country = await _readRepository.GetByIdAsync(id);
        if (country == null || country.IsDeleted)
            return Result<CountryDto>.Failure(MessageGenerator.NotFound("Country"), 404);

        var dto = _mapper.Map<CountryDto>(country);
        return Result<CountryDto>.Success(dto);
    }

    public async Task<Result<PagedResult<CountryDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = _readRepository
            .GetAll()
            .OrderBy(x => x.Name)
            .ProjectTo<CountryDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<CountryDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<CountryDto>>.Success(pagedResult);
    }

    public async Task<Result<CountryDto>> CreateAsync(CreateCountryDto dto)
    {
        var duplicate = await _readRepository.GetSingleAsync(
            x => x.Name.ToLower() == dto.Name.ToLower() || x.Code.ToLower() == dto.Code.ToLower());

        if (duplicate is not null)
            return Result<CountryDto>.Failure(MessageGenerator.AlreadyExists("Country"), 409);

        var country = _mapper.Map<Country>(dto);

        await _writeRepository.AddAsync(country);
        await _writeRepository.SaveAsync();

        var resultDto = _mapper.Map<CountryDto>(country);
        return Result<CountryDto>.Success(resultDto, MessageGenerator.CreationSuccess("Country"));
    }

    public async Task<Result<bool>> UpdateAsync(UpdateCountryDto dto)
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
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.UpdateSuccess("Country"));
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var country = await _readRepository.GetByIdAsync(id);
        if (country == null || country.IsDeleted)
            return Result<bool>.Failure(MessageGenerator.NotFound("Country"), 404);

        country.IsDeleted = true;
        _writeRepository.Update(country);
        await _writeRepository.SaveAsync();

        return Result<bool>.Success(true, MessageGenerator.DeletionSuccess("Country"));
    }
}
