namespace Weather.Aggregator.Application.Interfaces.Providers;

public interface ICoordinatesProvider
{
    (double Latitude, double Longitude) GetCoordinatesAsync(string city, CancellationToken cancellationToken);

    IReadOnlyList<string> GetCitiesAsync();
}