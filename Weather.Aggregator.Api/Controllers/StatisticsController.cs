using Microsoft.AspNetCore.Mvc;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Api.Controllers;

public class StatisticsController(
    ILogging<StatisticsController> logging, 
    ICorrelationIdContext correlationIdContext) 
    : BaseController<StatisticsController>(logging, correlationIdContext)
{

    [HttpGet]
    public async Task<IActionResult> GetStatisticsAsync(
        [FromServices] IStatisticsService statisticsService,
        CancellationToken cancellationToken)
    {
        return await HandleRequestAsync(
            request: async () =>
            {
                var stats = await statisticsService.GetStatisticsAsync();
                return Ok(stats);
            });
    }
}