using Application.Core;

namespace Application.Params;

public class TeamFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public string? CountryId { get; set; }
    public string? SeasonId { get; set; }
    public string? LeagueId { get; set; }
}
