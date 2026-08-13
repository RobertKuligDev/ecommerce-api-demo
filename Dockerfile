# Build stage: full SDK with tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only csproj first for better layer caching
COPY src/EcomDemo.Domain/EcomDemo.Domain.csproj src/EcomDemo.Domain/
COPY src/EcomDemo.Application/EcomDemo.Application.csproj src/EcomDemo.Application/
COPY src/EcomDemo.Infrastructure/EcomDemo.Infrastructure.csproj src/EcomDemo.Infrastructure/
COPY src/EcomDemo.Api/EcomDemo.Api.csproj src/EcomDemo.Api/
COPY tests/EcomDemo.Tests/EcomDemo.Tests.csproj tests/EcomDemo.Tests/
COPY EcomDemo.sln .

RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet build -c Release --no-restore --warnaserror
RUN dotnet test -c Release --no-build --no-restore
RUN dotnet publish src/EcomDemo.Api/EcomDemo.Api.csproj -c Release -o /app --no-build

# Runtime stage: minimal image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

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