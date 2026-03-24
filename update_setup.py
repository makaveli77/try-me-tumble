import os

with open('/Users/almirhrvat/Desktop/projects/try-me-tumble/setup.sh', 'r') as f:
    sh_content = f.read()

sh_content = sh_content.replace(
    '''echo "🌱 Seeding 20,000 random websites into PostgreSQL and Redis..."
curl -s -X POST "http://localhost:5200/api/Websites/seed?count=20000" > /dev/null''',
    '''# Source .env file if it exists
if [ -f .env ]; then
  source .env
fi

# Default seed count to 20000 if not set in .env
SEED_COUNT=${SEED_COUNT:-20000}

echo "🌱 Seeding $SEED_COUNT random websites into PostgreSQL and Redis..."
curl -s -X POST "http://localhost:5200/api/Websites/seed?count=$SEED_COUNT" > /dev/null'''
)

with open('/Users/almirhrvat/Desktop/projects/try-me-tumble/setup.sh', 'w') as f:
    f.write(sh_content)

print("Updated setup.sh")
