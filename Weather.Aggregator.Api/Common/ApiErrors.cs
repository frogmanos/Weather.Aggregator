using Weather.Aggregator.Application.Common;

namespace Weather.Aggregator.Api.Common
{
    public static class ApiErrors
    {
        public static readonly ApiError CityIsRequired =
            new ApiError(nameof(CityIsRequired), "City parameter is required.", ErrorType.Validation.ToString());
    }
}
