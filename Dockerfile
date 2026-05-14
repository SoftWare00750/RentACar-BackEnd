# ─────────────────────────────────────────────────────────────────────────────
# Stage 1 — Build
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (layer-cache friendly)
COPY RentACar.API.csproj ./
RUN dotnet restore RentACar.API.csproj

# Copy the rest of the source and publish in Release mode
COPY . ./
RUN dotnet publish RentACar.API.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

# ─────────────────────────────────────────────────────────────────────────────
# Stage 2 — Runtime (much smaller image)
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for the health-check probe
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published output from build stage
COPY --from=build /app/publish .

# Create the uploads directory that the app writes car images into.
# On Render (and Docker Compose) this is mounted as a volume so data survives
# container restarts.
RUN mkdir -p /app/wwwroot/uploads

# ── Non-root user for security ────────────────────────────────────────────────
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
RUN chown -R appuser:appgroup /app
USER appuser

# Render / Docker always route HTTP traffic; the app listens on $PORT (default 8080)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health check — Render and compose will call this every 30 s
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RentACar.API.dll"]
