namespace Application.Services;

public interface IFootballDataImporterService
{
    Task ImportInitialDataAsync(CancellationToken cancellationToken);
    Task ImportMatchesAsync(CancellationToken cancellationToken);
}