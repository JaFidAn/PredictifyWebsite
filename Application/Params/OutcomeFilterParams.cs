using Application.Core;

namespace Application.Params;

public class OutcomeFilterParams : PaginationParams
{
    public string? Name { get; set; }
}
