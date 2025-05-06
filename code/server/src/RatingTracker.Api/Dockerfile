# Stage 1: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files for better caching
COPY RatingTracker.Api/RatingTracker.Api.csproj RatingTracker.Api/
COPY RatingTracker.Domain/RatingTracker.Domain.csproj RatingTracker.Domain/
COPY RatingTracker.Application/RatingTracker.Application.csproj RatingTracker.Application/
COPY RatingTracker.Infrastructure/RatingTracker.Infrastructure.csproj RatingTracker.Infrastructure/

# Restore dependencies
RUN dotnet restore RatingTracker.Api/RatingTracker.Api.csproj

# Copy everything else
COPY . .

# Build project
WORKDIR /src/RatingTracker.Api
RUN dotnet build -c $BUILD_CONFIGURATION --no-restore -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy published output
COPY --from=publish /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "RatingTracker.Api.dll"]