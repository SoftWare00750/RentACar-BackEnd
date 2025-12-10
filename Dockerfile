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

# Copy the rest of the source code
COPY . .

# Build and publish
WORKDIR "/src/RentACar.API"
RUN dotnet publish "RentACar.API.csproj" -c Release -o /publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy published output from build stage
COPY --from=build /publish .

# Start the application
ENTRYPOINT ["dotnet", "RentACar.API.dll"]
