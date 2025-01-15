FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5300
ENV ASPNETCORE_URLS=http://+:5300

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GameService/GameService.csproj", "src/GameService/"]
COPY ["src/BuildingBlocks/Shared/Shared.csproj", "src/BuildingBlocks/Shared/"]
RUN dotnet restore "src/GameService/GameService.csproj"
COPY . .
WORKDIR "/src/src/GameService"
RUN dotnet build "GameService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameService.dll"]
