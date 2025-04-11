using Application.Core;

namespace Application.Params;

public class TeamOutcomeStreakFilterParams : PaginationParams
{
    public int? TeamId { get; set; }
    public int? OutcomeId { get; set; }
}
