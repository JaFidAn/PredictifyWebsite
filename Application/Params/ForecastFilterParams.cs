using Application.Core;

namespace Application.Params;

public class ForecastFilterParams : PaginationParams
{
    public bool? IsCorrect { get; set; }
    public int? LeagueId { get; set; }
    public int? TeamId { get; set; }
    public int? OutcomeId { get; set; }
    public DateTime? StartDate { get; set; }  
    public DateTime? EndDate { get; set; }
}