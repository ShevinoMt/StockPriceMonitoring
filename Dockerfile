# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8081
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/StockPriceMonitoring.Api/StockPriceMonitoring.Api.csproj", "API/StockPriceMonitoring.Api/"]
COPY ["Repositories/StockPricingMonitoring.Repositories.EF/StockPricingMonitoring.Repositories.EF.csproj", "Repositories/StockPricingMonitoring.Repositories.EF/"]
COPY ["Models/StockPriceMonitoring.Core/StockPriceMonitoring.Core.csproj", "Models/StockPriceMonitoring.Core/"]
COPY ["Repositories/StockPricingMonitoring.Repositories.Core/StockPricingMonitoring.Repositories.Core.csproj", "Repositories/StockPricingMonitoring.Repositories.Core/"]
COPY ["Services/StockPricingMonitoring.Services/StockPricingMonitoring.Services.csproj", "Services/StockPricingMonitoring.Services/"]
COPY ["Services/StockPricingMonitoring.Services.Core/StockPricingMonitoring.Services.Core.csproj", "Services/StockPricingMonitoring.Services.Core/"]
RUN dotnet restore "./API/StockPriceMonitoring.Api/StockPriceMonitoring.Api.csproj"
COPY . .
WORKDIR "/src/API/StockPriceMonitoring.Api"
RUN dotnet build "./StockPriceMonitoring.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StockPriceMonitoring.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockPriceMonitoring.Api.dll"]