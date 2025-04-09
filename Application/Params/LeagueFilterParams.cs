using Application.Core;

namespace Application.Params;

public class LeagueFilterParams : PaginationParams
{
    public int? CountryId { get; set; }
    public int? CompetitionId { get; set; }
    public string? Name { get; set; }
}
