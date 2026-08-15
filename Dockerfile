# Build stage: full SDK with tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything first to ensure all csproj and solution files are present for restore
COPY . .

RUN dotnet restore EcomDemo.sln

# Build, test and publish
RUN dotnet build EcomDemo.sln -c Release --no-restore --warnaserror
RUN dotnet test tests/EcomDemo.Tests/EcomDemo.Tests.csproj -c Release --no-build --no-restore
RUN dotnet publish src/EcomDemo.Api/EcomDemo.Api.csproj -c Release -o /app --no-build

# Runtime stage: minimal image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks (Debian/Ubuntu version)
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Security: run as non-root
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

COPY --from=build /app .
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EcomDemo.Api.dll"]