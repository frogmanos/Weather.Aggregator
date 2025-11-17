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