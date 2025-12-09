# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["RentACar.API/RentACar.API.csproj", "RentACar.API/"]
COPY ["RentACar.Business/RentACar.Business.csproj", "RentACar.Business/"]
COPY ["RentACar.Core/RentACar.Core.csproj", "RentACar.Core/"]
COPY ["RentACar.DataAccess/RentACar.DataAccess.csproj", "RentACar.DataAccess/"]
COPY ["RentACar.Entities/RentACar.Entities.csproj", "RentACar.Entities/"]

# Restore dependencies
RUN dotnet restore "RentACar.API/RentACar.API.csproj"

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/RentACar.API"
RUN dotnet publish "RentACar.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

# Copy published files
COPY --from=build /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "RentACar.API.dll"]