# ─────────────────────────────────────────────────────────────────────────────
# Stage 1 — Build
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (layer-cache friendly)
COPY RentACar.API.csproj ./
RUN dotnet restore RentACar.API.csproj

# Copy source. Remove any legacy multi-project folders that reference
# missing assemblies (e.g. RentACar.DataAccess/EfRentalDal.cs) before publish.
COPY . ./
RUN rm -rf RentACar.DataAccess RentACar.Core RentACar.Entities RentACar.Business

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

# Pre-create directories the app writes to at runtime.
# /data  → Render Persistent Disk mount point for the SQLite database.
#          The volume mount overrides this empty dir; we create it here so
#          the app does not crash if the volume is not attached (e.g. local run).
# /app/wwwroot/uploads → car image storage (also volume-mounted on Render).
RUN mkdir -p /data /app/wwwroot/uploads

# ── Non-root user for security ────────────────────────────────────────────────
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
RUN chown -R appuser:appgroup /app /data
USER appuser

# Render / Docker route HTTP; the app listens on port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Health check — Render and compose call this every 30 s
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RentACar.API.dll"]