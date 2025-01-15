FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5400
ENV ASPNETCORE_URLS=http://+:5400

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BackgroundServiceJobs/BackgroundServiceJobs.csproj", "src/BackgroundServiceJobs/"]
COPY ["src/BuildingBlocks/Shared/Shared.csproj", "src/BuildingBlocks/Shared/"]
RUN dotnet restore "src/BackgroundServiceJobs/BackgroundServiceJobs.csproj"
COPY . .
WORKDIR "/src/src/BackgroundServiceJobs"
RUN dotnet build "BackgroundServiceJobs.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BackgroundServiceJobs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackgroundServiceJobs.dll"]
