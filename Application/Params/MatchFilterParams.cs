using Application.Core;

namespace Application.Params;

public class MatchFilterParams : PaginationParams
{
    public int? TeamId { get; set; }
    public int? SeasonId { get; set; }
    public int? LeagueId { get; set; }
    public DateTime? MatchDateFrom { get; set; }
    public DateTime? MatchDateTo { get; set; }
    public bool? IsCompleted { get; set; }
}
