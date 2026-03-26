#!/bin/bash

# Exit on any error
set -e

echo "🌊 Starting TryMeTumble Full Reset & Setup..."

# 1. Hard Reset Infrastructure
echo "🗑  Stopping and removing existing containers and volumes..."
docker-compose down -v --remove-orphans || true

echo "🐳 Starting fresh PostgreSQL and Redis (Port 6381 mapping)..."
docker-compose up -d db redis

# 2. Wait for Infrastructure
echo "⏳ Waiting for Database and Redis to be ready..."
# Simple wait loop for Postgres
until docker exec try-me-tumble-db pg_isready -U postgres; do
  echo "Postgres is unavailable - sleeping"
  sleep 2
done

# 3. Clean and Restore
echo "🧹 Cleaning previous builds..."
dotnet clean
rm -rf bin/ obj/

echo "📦 Restoring .NET dependencies..."
dotnet restore

# 4. Apply Migrations
echo "🏗  Applying Database Migrations..."
dotnet run --project src/DatabaseUpgrader/TryMeTumble.DatabaseUpgrader.csproj

# 5. Start and Seed
echo "🚀 Starting API in background to seed data..."
# Run API in background, redirecting output to a log file
dotnet run --project TryMeTumble.csproj > api_setup.log 2>&1 &
API_PID=$!

echo "⏳ Waiting for API to start (Checking http://localhost:5202/health)..."
# Wait for health check to pass
max_attempts=30
attempt=0
until (curl --output /dev/null --silent --head --fail http://localhost:5202/health) || [ $attempt -eq $max_attempts ]; do
    printf '.'
    attempt=$((attempt+1))
    sleep 2
done

if [ $attempt -eq $max_attempts ]; then
    echo "❌ API failed to start. Check api_setup.log"
    cat api_setup.log
    kill $API_PID || true
    exit 1
fi

echo -e "\n🌱 Seeding 1000 random websites..."
curl -s -X POST "http://localhost:5202/api/Websites/seed?count=1000" > /dev/null

echo "✅ Seeding complete!"
echo "🛑 Stopping seeding background process..."
# Kill the specific background process group to ensure it's fully stopped
kill -TERM -$API_PID 2>/dev/null || kill $API_PID || true
# Wait a moment for the port to be released
sleep 3

# 6. Final Launch
echo "🎉 Setup Complete!"
echo "📱 Access the frontend at: http://localhost:5202/"
echo "📄 Swagger UI: http://localhost:5202/swagger"

echo "🚀 Launching API..."
dotnet run --project TryMeTumble.csproj
