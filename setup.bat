@echo off
setlocal

echo 🌊 Starting TryMeTumble Full Reset ^& Setup...

echo 🗑  Stopping and removing existing containers and volumes...
docker-compose down -v --remove-orphans

echo 🐳 Starting fresh PostgreSQL and Redis (Port 6381 mapping)...
docker-compose up -d db redis

echo ⏳ Waiting for Database and Redis to be ready...
:postgres_wait
docker exec try-me-tumble-db pg_isready -U postgres >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo Postgres is unavailable - sleeping
    timeout /t 2 /nobreak >nul
    goto postgres_wait
)

echo 🧹 Cleaning previous builds...
dotnet clean
if exist bin rd /s /q bin
if exist obj rd /s /q obj

echo 📦 Restoring .NET dependencies...
dotnet restore

echo 🏗  Applying Database Migrations...
dotnet run --project src\DatabaseUpgrader\TryMeTumble.DatabaseUpgrader.csproj

echo 🚀 Starting API in background to seed data...
set "API_PORT=5202"
start /b dotnet run --project TryMeTumble.csproj --launch-profile http > api_setup.log 2>&1
echo ⏳ Waiting for API to start (Checking http://localhost:%API_PORT%/health)...
set "max_attempts=30"
set "attempt=0"
:api_wait
curl --output nul --silent --head --fail http://localhost:%API_PORT%/health
if %ERRORLEVEL% neq 0 (
    set /a attempt+=1
    if %attempt% geq %max_attempts% (
        echo ❌ API failed to start. Check api_setup.log
        exit /b 1
    )
    set /p="." <nul
    timeout /t 2 /nobreak >nul
    goto api_wait
)

echo.
echo 🌱 Seeding 1000 random websites...
curl -s -X POST "http://localhost:%API_PORT%/api/Websites/seed?count=1000" > nul

echo ✅ Seeding complete!
echo 🛑 Stopping seeding background process...
taskkill /IM TryMeTumble.exe /F 2>nul
taskkill /IM dotnet.exe /F 2>nul

echo 🚀 Launching API...
echo 🎉 Setup Complete!

dotnet run --project TryMeTumble.csproj --launch-profile http

echo 📱 Access the frontend at: http://localhost:%API_PORT%/
echo 📄 Swagger UI: http://localhost:%API_PORT%/swagger
