using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;
using Weather.Aggregator.Application.Logging;

namespace Weather.Aggregator.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController<TController>(
    ILogging<TController> logging,
    ICorrelationIdContext correlationIdContext) 
    : ControllerBase where TController : class
{       
    protected ILogging<TController> Logging => logging;

    protected ICorrelationIdContext CorrelationIdContext => correlationIdContext;
      
    protected static IActionResult CreateResponse(HttpStatusCode statusCode, object? value = default) => new ObjectResult(value) { StatusCode = (int)statusCode };

    protected async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> request,
        [CallerMemberName] string callingMethod = "")
    {
        using (logging.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationIdContext.GetCorrelationId }))
        {
            try
            {
                return await request();
            }
            catch (Exception ex)
            {
                logging.Error(
                    message: $"Exception occurred in {callingMethod}",
                    exception: ex,
                    memberName: callingMethod);

                return CreateResponse(HttpStatusCode.InternalServerError, "Exception occurred");
            }
        }
    }
}