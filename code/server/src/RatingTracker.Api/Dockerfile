FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only .csproj files first (for layer caching)
COPY ["RatingTracker.Api/RatingTracker.Api.csproj", "RatingTracker.Api/"]
COPY ["RatingTracker.Domain/RatingTracker.Domain.csproj", "RatingTracker.Domain/"]
COPY ["RatingTracker.Application/RatingTracker.Application.csproj", "RatingTracker.Application/"]
COPY ["RatingTracker.Infrastructure/RatingTracker.Infrastructure.csproj", "RatingTracker.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "RatingTracker.Api/RatingTracker.Api.csproj"

# Copy full source
COPY . .

# Build
WORKDIR /src/RatingTracker.Api
RUN dotnet build "RatingTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "RatingTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RatingTracker.Api.dll"]