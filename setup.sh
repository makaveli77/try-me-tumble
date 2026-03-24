#!/bin/bash

echo "� Starting try-me-tumble Clone Setup..."

# 1. Start Infrastructure
echo "🐳 Starting PostgreSQL and Redis via Docker..."
docker-compose up -d db redis

# 2. Restore Dependencies
echo "📦 Restoring .NET dependencies..."
dotnet restore

# 3. Wait for Database
echo "⏳ Waiting for Database to be ready..."
sleep 5

# 4. Apply Migrations
echo "🏗  Applying DbUp Database Migrations..."
dotnet run --project src/DatabaseUpgrader/TryMeTumble.DatabaseUpgrader.csproj

# 5. Build and Start API in background to Seed Data
echo "🚀 Starting temporary API to seed mock data..."
dotnet run --project TryMeTumble.csproj -c Release &
API_PID=$!
sleep 5 # Wait for Kestrel to wire up

# Source .env file if it exists
if [ -f .env ]; then
  source .env
fi

# Default seed count to 20000 if not set in .env
SEED_COUNT=${SEED_COUNT:-20000}

echo "🌱 Seeding $SEED_COUNT random websites into PostgreSQL and Redis..."
curl -s -X POST "http://localhost:5200/api/Websites/seed?count=$SEED_COUNT" > /dev/null
echo -e "\n✅ Seeding complete!"

# Shut down temporary background API
kill $API_PID

# 6. Final Step
echo "🎉 Full Setup Complete!"
echo "🏃 Run 'dotnet run' to start the application natively,"
echo "📄 或者 'docker-compose up -d' for the containerized version."

