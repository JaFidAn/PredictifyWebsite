using Application.Core;

namespace Application.Params;

public class TeamFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public int? CountryId { get; set; }
    public int? SeasonId { get; set; }
    public int? LeagueId { get; set; }
}
