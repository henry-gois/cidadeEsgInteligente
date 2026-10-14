# Build stage: restore and publish the .NET 8 API.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Esg.Api/Esg.Api.csproj src/Esg.Api/
RUN dotnet restore src/Esg.Api/Esg.Api.csproj

COPY src/Esg.Api/ src/Esg.Api/
RUN dotnet publish src/Esg.Api/Esg.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# Runtime stage: smaller ASP.NET image and non-root application user.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Esg.Api.dll"]
