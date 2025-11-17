using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Weather.Aggregator.Application.Common;
using Weather.Aggregator.Application.DataTransferObjects.Common;
using Weather.Aggregator.Application.DataTransferObjects.FetchWeather;
using Weather.Aggregator.Application.Interfaces.Api;
using Weather.Aggregator.Application.Interfaces.Providers;
using Weather.Aggregator.Application.Interfaces.Services;
using Weather.Aggregator.Application.Logging;
using Weather.Aggregator.Application.Services.FetchWeather;

namespace Weather.Aggregator.Tests.Application.Services
{
    public class FetchWeatherServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnAllClientsData_WhenSucceed()
        {
            // Arrange

            var testProvider = TestProvider
                .Initialize()
                .MockCoordinatorProvider(32.1234,23.4321)
                .MockOpenWeatherClient()
                .MockWeatherApiClient()
                .MockWeatherBitClient();
                
            var fetchWeatherService = testProvider.CreateService();

            // Act

            var result = await fetchWeatherService.ExecuteAsync(TestProvider.GetFetchWeatherQuery(), CancellationToken.None);

            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Athens", result.Value.City);
            Assert.Equal(3, result.Value.WeatherSources.Count);

            testProvider
                .AssertOpenWeatherCalledOnce()
                .AssertWeatherApiCalledOnce()
                .AssertWeatherBitCalledOnce();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFailed_WhenCityNotFound()
        {
            // Arrange

            var testProvider = TestProvider
                .Initialize()
                .MockCoordinatorProvider(0, 0);

            var fetchWeatherService = testProvider.CreateService();

            // Act

            var result = await fetchWeatherService.ExecuteAsync(TestProvider.GetFetchWeatherQuery(), CancellationToken.None);

            // Assert

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.HasErrors);
            Assert.Equal(result.Errors.First(), ApplicationErrors.InvalidLocationInput);            
        }

        private class TestProvider
        {
            private IFetchWeatherService _fetchWeatherService;
            private readonly IWeatherClient _openWeatherClientMock;
            private readonly IWeatherClient _weatherApiClientMock;
            private readonly IWeatherClient _weatherBitClientMock;
            private readonly ICoordinatesProvider _coordinatesProviderMock;
            private readonly ILogging<FetchWeatherService> _loggingMock;

            private TestProvider()
            {
                _openWeatherClientMock = Substitute.For<IWeatherClient>();
                _weatherApiClientMock = Substitute.For<IWeatherClient>();
                _weatherBitClientMock = Substitute.For<IWeatherClient>();                
                _loggingMock = Substitute.For<ILogging<FetchWeatherService>>();
                _coordinatesProviderMock = Substitute.For<ICoordinatesProvider>();
            }

            public static TestProvider Initialize() => new();

            public TestProvider AssertOpenWeatherCalledOnce()
            {
                _openWeatherClientMock
                    .Received(1)
                    .GetWeatherDataAsync(
                        Arg.Any<double>(),
                        Arg.Any<double>(),
                        Arg.Any<CancellationToken>());

                return this;
            }

            public TestProvider AssertWeatherBitCalledOnce()
            {
                _weatherBitClientMock
                    .Received(1)
                    .GetWeatherDataAsync(
                        Arg.Any<double>(),
                        Arg.Any<double>(),
                        Arg.Any<CancellationToken>());

                return this;
            }

            public TestProvider AssertWeatherApiCalledOnce()
            {
                _weatherApiClientMock
                    .Received(1)
                    .GetWeatherDataAsync(
                        Arg.Any<double>(),
                        Arg.Any<double>(),
                        Arg.Any<CancellationToken>());

                return this;
            }

            public TestProvider MockCoordinatorProvider(double latitude, double longitude)
            {
                _coordinatesProviderMock
                    .GetCoordinatesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns((latitude, longitude));

                return this;
            }

            public TestProvider MockOpenWeatherClient()
            {
                _openWeatherClientMock.GetWeatherDataAsync(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<CancellationToken>())
                    .Returns(Result.Success(new CurrentWeather
                    {
                        SourceName = "OpenWeatherClient",
                        TemperatureC = 28.5,
                        Condition = "Sunny",
                        FeelsLikeC = 29.4,
                        TimestampUtc = DateTime.UtcNow
                    }));

                return this;
            }

            public TestProvider MockWeatherApiClient()
            {
                _weatherApiClientMock.GetWeatherDataAsync(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<CancellationToken>())
                    .Returns(Result.Success(new CurrentWeather
                    {
                        SourceName = "WeatherApiClient",
                        TemperatureC = 29.5,
                        Condition = "Dry",
                        FeelsLikeC = 30.4,
                        TimestampUtc = DateTime.UtcNow
                    }));

                return this;
            }

            public TestProvider MockWeatherBitClient()
            {
                _weatherBitClientMock.GetWeatherDataAsync(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<CancellationToken>())
                    .Returns(Result.Success(new CurrentWeather
                    {
                        SourceName = "WeatherBitClient",
                        TemperatureC = 27.8,
                        Condition = "Sunny",
                        FeelsLikeC = 39.2,
                        TimestampUtc = DateTime.UtcNow
                    }));

                return this;
            }

            public IFetchWeatherService CreateService()
            {
                IEnumerable<IWeatherClient> weatherClients = [
                    _openWeatherClientMock,
                    _weatherApiClientMock,
                    _weatherBitClientMock
                ];

                _fetchWeatherService = new FetchWeatherService(
                    weatherClients: weatherClients,
                    coordinateProvider: _coordinatesProviderMock,
                    logging: _loggingMock,
                    memoryCache: new MemoryCache(new MemoryCacheOptions())
                );

                return _fetchWeatherService;
            }

            public static FetchWeatherQuery GetFetchWeatherQuery(string? sortBy = null) => new("Athens", sortBy);
        }
    }
}
