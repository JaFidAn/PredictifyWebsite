using Application.DTOs.AuditLogs;
using Application.DTOs.Competitions;
using Application.DTOs.Countries;
using Application.DTOs.Forecasts;
using Application.DTOs.Leagues;
using Application.DTOs.Matches;
using Application.DTOs.MatchOutcomes;
using Application.DTOs.MatchTeamSeasonLeagues;
using Application.DTOs.Outcomes;
using Application.DTOs.Seasons;
using Application.DTOs.TeamOutcomeStreaks;
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

        // ✅ Match
        CreateMap<Match, MatchDto>()
            .ForMember(dest => dest.Team1Name, opt => opt.MapFrom(src => src.Team1.Name))
            .ForMember(dest => dest.Team2Name, opt => opt.MapFrom(src => src.Team2.Name))
            .ForMember(dest => dest.SeasonId, opt => opt.MapFrom(src =>
                src.MatchTeamSeasonLeagues.FirstOrDefault()!.SeasonId)) 
            .ForMember(dest => dest.LeagueId, opt => opt.MapFrom(src =>
                src.MatchTeamSeasonLeagues.FirstOrDefault()!.LeagueId)); 
        
        // ✅ MatchTeamSeasonLeague
        CreateMap<MatchTeamSeasonLeague, MatchTeamSeasonLeagueDto>()
            .ForMember(dest => dest.SeasonName, opt => opt.MapFrom(src => src.Season.Name))
            .ForMember(dest => dest.LeagueName, opt => opt.MapFrom(src => src.League.Name));

        // ✅ MatchOutcome
        CreateMap<MatchOutcome, MatchOutcomeDto>()
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
            .ForMember(dest => dest.OutcomeName, opt => opt.MapFrom(src => src.Outcome.Name));

        // ✅ TeamOutcomeStreak
        CreateMap<TeamOutcomeStreak, TeamOutcomeStreakDto>()
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
            .ForMember(dest => dest.OutcomeName, opt => opt.MapFrom(src => src.Outcome.Name));
        
        // ✅ Forecast
        CreateMap<Forecast, ForecastDto>()
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
            .ForMember(dest => dest.OutcomeName, opt => opt.MapFrom(src => src.Outcome.Name));
    }
}
