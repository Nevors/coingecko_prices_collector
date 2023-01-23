#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/CoingeckoPrices.WebApi/CoingeckoPrices.WebApi.csproj", "Services/CoingeckoPrices.WebApi/"]
COPY ["Common/MessageBroker/Kafka.Subscriber/Kafka.Subscriber.csproj", "Common/MessageBroker/Kafka.Subscriber/"]
COPY ["Common/MessageBroker/Kafka/Kafka.csproj", "Common/MessageBroker/Kafka/"]
COPY ["Common/Configuration/Configuration.csproj", "Common/Configuration/"]
COPY ["Common/MessageBroker/MessageBroker.Contracts/MessageBroker.Contracts.csproj", "Common/MessageBroker/MessageBroker.Contracts/"]
COPY ["Common/MessageBroker/Coingecko.Events.Contracts/Coingecko.Events.Contracts.csproj", "Common/MessageBroker/Coingecko.Events.Contracts/"]
RUN dotnet restore "Services/CoingeckoPrices.WebApi/CoingeckoPrices.WebApi.csproj"
COPY . .
WORKDIR "/src/Services/CoingeckoPrices.WebApi"
RUN dotnet build "CoingeckoPrices.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoingeckoPrices.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoingeckoPrices.WebApi.dll"]