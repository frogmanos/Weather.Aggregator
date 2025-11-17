using Weather.Aggregator.Api.Common;
using Weather.Aggregator.Application.Common;

namespace Weather.Aggregator.Api.Extensions
{
    public static class ApiResponseExtension
    {        
        public static async Task<ApiResponse<TContracts>> ToApiResponse<T, TContracts>(this Task<Result<T>> resultTask, Func<T, TContracts> funcMapToContracts)
        {
            var result = await resultTask;

            if (result.IsSuccess)
            {
                var contracts = funcMapToContracts(result.Value!);

                return ApiResponse.Success<TContracts>(contracts!);
            }

            var errors = result.Errors
                .Select(e => new ApiError(
                    e.Code,
                    e.Message,
                    e.ErrorType.ToString()))
                .ToArray();

            return ApiResponse.Failure<TContracts>(errors);
        }

        public static async Task<ApiResponse<T>> ToApiResponse<T>(this Task<Result<T>> resultTask)
        {
            var result = await resultTask;

            if (result.IsSuccess)
            {
                return ApiResponse.Success<T>(result.Value!);
            }

            var errors = result.Errors
                .Select(e => new ApiError(
                    e.Code,
                    e.Message,
                    e.ErrorType.ToString()))
                .ToArray();

            return ApiResponse.Failure<T>(errors);
        }
    }
}
