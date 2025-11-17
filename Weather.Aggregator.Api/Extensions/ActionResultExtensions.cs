using Microsoft.AspNetCore.Mvc;
using Weather.Aggregator.Api.Common;

namespace Weather.Aggregator.Api.Extensions;

public static class ActionResultExtensions
{
    public static async Task<IActionResult> ToActionResult<TData>(this Task<ApiResponse<TData>> resultTask)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            return new BadRequestObjectResult(result);
        }

        return new OkObjectResult(result);
    }

    public static async Task<IActionResult> ToActionResult(this Task<ApiResponse> resultTask)
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            return new BadRequestObjectResult(result);
        }

        return new OkObjectResult(result);
    }
}