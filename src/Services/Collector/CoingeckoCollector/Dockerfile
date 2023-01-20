#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Collector/CoingeckoCollector/CoingeckoCollector.csproj", "Services/Collector/CoingeckoCollector/"]
COPY ["Common/ExternalApi/Coingecko.ExternalApi/Coingecko.Api.csproj", "Common/ExternalApi/Coingecko.ExternalApi/"]
COPY ["Common/ExternalApi/Coingecko.ExternalApi.Contracts/Coingecko.Api.Contracts.csproj", "Common/ExternalApi/Coingecko.ExternalApi.Contracts/"]
RUN dotnet restore "Services/Collector/CoingeckoCollector/CoingeckoCollector.csproj"
COPY . .
WORKDIR "/src/Services/Collector/CoingeckoCollector"
RUN dotnet build "CoingeckoCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoingeckoCollector.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoingeckoCollector.dll"]