using Application.Core;

namespace Application.Params;

public class ForecastFilterParams : PaginationParams
{
    public bool? IsCorrect { get; set; }
    public int? MatchId { get; set; }
    public int? TeamId { get; set; }
    public int? OutcomeId { get; set; }
}