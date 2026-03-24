@echo off
echo � Starting try-me-tumble Clone Setup for Windows...

REM 1. Start Infrastructure
echo 🐳 Starting PostgreSQL and Redis via Docker...
docker-compose up -d db redis

REM 2. Restore Dependencies
echo 📦 Restoring .NET dependencies...
dotnet restore

REM 3. Wait for DB to be healthy
echo ⏳ Waiting 10s for Database to initialize...
timeout /t 10 /nobreak >nul

REM 4. Apply Migrations
echo 🔄 Running DbUp Migrations...
dotnet run --project src/DatabaseUpgrader/TryMeTumble.DatabaseUpgrader.csproj

REM 5. Seed Data via background API
echo 🚀 Starting API in background to seed mock data...
start /B "TryMeTumbleAPI" dotnet run --project TryMeTumble.csproj -c Release

echo ⏳ Waiting 6s for API to start up...
timeout /t 6 /nobreak >nul

REM Default seed count
set SEED_COUNT=20000

REM Source .env file if it exists (very basic parsing)
if exist .env (
    for /f "tokens=1,2 delims==" %%A in (.env) do (
        if "%%A"=="SEED_COUNT" set SEED_COUNT=%%B
    )
)

echo 🌱 Seeding %SEED_COUNT% random websites into PostgreSQL and Redis...
curl -s -X POST "http://localhost:5200/api/Websites/seed?count=%SEED_COUNT%" > nul
echo ✅ Seeding complete!

REM Kill the background API process
taskkill /F /IM TryMeTumble.exe /T >nul 2>&1

echo.
echo 🎉 Full Setup Complete!
echo 🚀 Run 'dotnet run' to start the application natively.
echo.
pause

