using Application.DTOs.AuditLogs;
using Application.DTOs.Competitions;
using Application.DTOs.Countries;
using Application.DTOs.Leagues;
using Application.DTOs.Outcomes;
using Application.DTOs.Seasons;
using Application.DTOs.Teams;
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

        // ✅ Competition
        CreateMap<Competition, CompetitionDto>();
        CreateMap<CreateCompetitionDto, Competition>();
        CreateMap<UpdateCompetitionDto, Competition>();

        // ✅ League
        CreateMap<League, LeagueDto>();
        CreateMap<CreateLeagueDto, League>();
        CreateMap<UpdateLeagueDto, League>();

        // ✅ Outcome
        CreateMap<Outcome, OutcomeDto>();
        CreateMap<CreateOutcomeDto, Outcome>();
        CreateMap<UpdateOutcomeDto, Outcome>();

        // ✅ Season
        CreateMap<Season, SeasonDto>();
        CreateMap<CreateSeasonDto, Season>();
        CreateMap<UpdateSeasonDto, Season>();

        // ✅ Team
        CreateMap<CreateTeamDto, Team>();
        CreateMap<UpdateTeamDto, Team>();

        CreateMap<Team, TeamDto>()
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));

        // ✅ TeamSeasonLeague
        CreateMap<TeamSeasonLeague, TeamSeasonLeagueDto>()
            .ForMember(dest => dest.SeasonName, opt => opt.MapFrom(src => src.Season.Name))
            .ForMember(dest => dest.LeagueName, opt => opt.MapFrom(src => src.League.Name));

        CreateMap<TeamSeasonLeagueCreateDto, TeamSeasonLeague>();
    }
}
