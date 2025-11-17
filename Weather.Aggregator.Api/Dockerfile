# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Weather.Aggregator.Api/Weather.Aggregator.Api.csproj", "Weather.Aggregator.Api/"]
COPY ["Weather.Aggregator.Application.Logging/Weather.Aggregator.Application.Logging.csproj", "Weather.Aggregator.Application.Logging/"]
COPY ["Weather.Aggregator.Application/Weather.Aggregator.Application.csproj", "Weather.Aggregator.Application/"]
COPY ["Weather.Aggregator.Application.Interfaces/Weather.Aggregator.Application.Interfaces.csproj", "Weather.Aggregator.Application.Interfaces/"]
COPY ["Weather.Aggregator.Application.Common/Weather.Aggregator.Application.Common.csproj", "Weather.Aggregator.Application.Common/"]
COPY ["Weather.Aggregator.Application.DataTransferObjects/Weather.Aggregator.Application.DataTransferObjects.csproj", "Weather.Aggregator.Application.DataTransferObjects/"]
COPY ["Weather.Aggregator.Infrastructure.Http.Client/Weather.Aggregator.Infrastructure.Http.Client.csproj", "Weather.Aggregator.Infrastructure.Http.Client/"]
COPY ["Weather.Aggregator.Infrastructure.Providers/Weather.Aggregator.Infrastructure.Providers.csproj", "Weather.Aggregator.Infrastructure.Providers/"]
RUN dotnet restore "./Weather.Aggregator.Api/Weather.Aggregator.Api.csproj"
COPY . .
WORKDIR "/src/Weather.Aggregator.Api"
RUN dotnet build "./Weather.Aggregator.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Weather.Aggregator.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather.Aggregator.Api.dll"]