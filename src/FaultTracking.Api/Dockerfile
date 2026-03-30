FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FaultTracking.Api/FaultTracking.Api.csproj FaultTracking.Api/
RUN dotnet restore FaultTracking.Api/FaultTracking.Api.csproj

COPY Core/ Core/
COPY Infrastructure/ Infrastructure/
COPY FaultTracking.Api/ FaultTracking.Api/

RUN dotnet build FaultTracking.Api/FaultTracking.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FaultTracking.Api/FaultTracking.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FaultTracking.Api.dll"]