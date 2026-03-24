# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["TryMeTumble.csproj", "./"]
RUN dotnet restore "TryMeTumble.csproj"

# Copy everything else and build app
COPY . .
RUN dotnet build "TryMeTumble.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TryMeTumble.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for healthchecks
RUN apk add --no-cache curl

COPY --from=publish /app/publish .

# Run as non-root user for security
USER $APP_UID

ENTRYPOINT ["dotnet", "TryMeTumble.dll"]

