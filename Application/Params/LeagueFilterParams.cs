using Application.Core;

namespace Application.Params;

public class LeagueFilterParams : PaginationParams
{
    public string? CountryId { get; set; }
    public string? CompetitionId { get; set; }
    public string? Name { get; set; }
}
