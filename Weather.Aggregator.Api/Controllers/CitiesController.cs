using Microsoft.AspNetCore.Mvc;
using Weather.Aggregator.Api.Common;
using Weather.Aggregator.Api.Extensions;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Api.Controllers;

public class CitiesController(
    ILogging<CitiesController> logging, 
    ICorrelationIdContext correlationIdContext) 
    : BaseController<CitiesController>(logging, correlationIdContext)
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status400BadRequest)]   
    public async Task<IActionResult> GetCitySuggestionsAsync(        
        [FromServices] IHelperService helperService,
        CancellationToken cancellationToken)
    {
        return await HandleRequestAsync(
            request: async () => await helperService.GetCitiesAsync()
                .ToApiResponse()
                .ToActionResult());
    }
}