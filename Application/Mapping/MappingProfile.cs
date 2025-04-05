using Application.DTOs.AuditLogs;
using Application.DTOs.Countries;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ✅ AuditLog
        CreateMap<AuditLog, AuditLogDto>();

        // ✅ Country
        CreateMap<Country, CountryDto>();
        CreateMap<CreateCountryDto, Country>();
        CreateMap<UpdateCountryDto, Country>();
    }
}
