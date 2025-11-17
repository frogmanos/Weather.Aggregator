using Microsoft.AspNetCore.Mvc;
using Weather.Aggregator.Api.Common;
using Weather.Aggregator.Api.Extensions;
using Weather.Aggregator.Api.Mappers;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;
using Weather.Aggregator.Contracts.Api.FetchWeather;

namespace Weather.Aggregator.Api.Controllers;

public class WeathersController(
    ILogging<WeathersController> logging, 
    ICorrelationIdContext correlationIdContext) 
    : BaseController<WeathersController>(logging, correlationIdContext)
{

    /// <summary>
    /// Gets weather information for a city.
    /// </summary>
    /// <param name="city">The name of the city.</param>
    /// <param name="sortBy">
    /// Sorting option. Possible values:
    /// <br/>temp, sort by temperature Ascending
    /// <br/>source, sort by Source Api
    /// <br/>temp_desc, sort by temperature Descending
    /// </param>
    [HttpGet("{city}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<FetchWeatherResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FetchWeatherResponse>), 400)]   
    public async Task<IActionResult> GetWeatherAsync(
        [FromRoute] string city,
        [FromQuery] string? sortBy,
        [FromServices] IFetchWeatherService fetchWeatherService,
        CancellationToken cancellationToken)
    {
        return await HandleRequestAsync(
            request: async () =>
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return BadRequest(ApiResponse.Failure<FetchWeatherResponse>(ApiErrors.CityIsRequired));
                }

                return await fetchWeatherService.ExecuteAsync(
                    new FetchWeatherQuery(
                        City: city,
                        SortBy: sortBy),
                    cancellationToken)
                .ToApiResponse(d => d.ToContract())
                .ToActionResult();
            });
    }
}