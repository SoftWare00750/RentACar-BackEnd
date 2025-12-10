# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["RentACar.API/RentACar.API.csproj", "RentACar.API/"]
COPY ["RentACar.Business/RentACar.Business.csproj", "RentACar.Business/"]
COPY ["RentACar.Core/RentACar.Core.csproj", "RentACar.Core/"]
COPY ["RentACar.DataAccess/RentACar.DataAccess.csproj", "RentACar.DataAccess/"]
COPY ["RentACar.Entities/RentACar.Entities.csproj", "RentACar.Entities/"]

# Restore
RUN dotnet restore "RentACar.API/RentACar.API.csproj"

# Copy all files
COPY . .

# Publish
WORKDIR "/src/RentACar.API"
RUN dotnet publish "RentACar.API.csproj" -c Release -o /publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "RentACar.API.dll"]
