# ─────────────────────────────────────────────────────────────────────────────
# Stage 1 — Build
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RentACar.API.csproj ./
RUN dotnet restore RentACar.API.csproj

# Remove any legacy multi-project folders before publish
COPY . ./
RUN rm -rf RentACar.DataAccess RentACar.Core RentACar.Entities RentACar.Business

RUN dotnet publish RentACar.API.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

# ─────────────────────────────────────────────────────────────────────────────
# Stage 2 — Runtime
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Uploads directory for car images (volume-mounted in Docker Compose)
RUN mkdir -p /app/wwwroot/uploads

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
RUN chown -R appuser:appgroup /app
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RentACar.API.dll"]