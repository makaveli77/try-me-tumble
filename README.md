# 🌀 Try Me Tumble - Random Website Discovery App

A professional, containerized ASP.NET Core Web API built with **.NET 10**, **PostgreSQL**, and **Redis**, following modern best practices (Clean Architecture, Repository Pattern, and DTOs).

## 📑 Summary

This project is a high-performance URL discovery service designed to mimic the classic "StumbleUpon" experience.

- **Architecture**: Domain-Driven Design principles with strict separation of concerns (Clean Architecture: Domain → Application → Infrastructure → API).
- **Performance**: High-speed lookups using **Redis** lists and caching for O(1) retrieval speed of websites.
- **Database Refinement**: Efficient PostgreSQL integration via **Entity Framework Core** strictly structured in isolated Configuration mappings.
- **Security**: Stateless **JWT Authentication** and secure BCrypt hashing for user accounts.
- **Validation & Mapping**: Strict internal mappers mapping domain entities to Data Transfer Objects (DTOs) to decouple internal data from API responses.
- **Dockerized**: Fully containerized environment with .NET 10, Database, and Redis services orchestrated via Docker Compose.

## 🌟 Key Features

- **Discover Randomly**: Instantly get a random website from the global database, prioritized by caching layers.
- **Categorized Discovery**: Supports blistering fast `O(1)` random website lookup natively scoped to specific categories via dynamically populated Redis cache queues.
- **Smart Data Seeding**: Rapid 20k+ site population via `Bogus`, efficiently piped into both PostgreSQL and segmented Redis queues perfectly tuned for immediate local development. **Includes a curated list of reliable, iframe-friendly websites (Wikipedia, TED, etc.) for a better initial experience.**
- **Upvote System**: Influence the discovery algorithm by upvoting websites you find interesting.
- **User Collections**: Save discovered websites to your personal profile for later viewing.
- **Internal Browser Experience**: Tumble through websites directly within the app using the integrated discovery frame with failure detection and easy skip options.
- **Smart Caching**: Uses Redis to cache candidate lists for discovery, minimizing database load during high traffic.
- **Stateless Auth**: Secure registration and login flow returning industry-standard JWT tokens.
- **Categorization & Tags**: Group websites by dynamic categories and flexible tags for precise discovery.
- **Sorting Algorithm Worker**: A robust `.NET BackgroundService` that runs periodically to score websites `(upvotes * 2) + (saves * 3) - (reports * 10)` and eagerly rebuilds Redis queues.
- **Content Moderation**: Flag and report system to keep the site safe.
- **Data Integrity**: Enforces strict URL uniqueness and implements paginated API responses for large datasets.

---

## 🚀 Getting Started

Follow these steps to get the project running locally in minutes.

### Prerequisites
- **Docker** & **Docker Compose**
- **.NET 10 SDK** (if running outside Docker)

### 1. Automatic Setup (Recommended)
You can set up the entire project using our automated scripts.

**For Mac/Linux:**
```bash
chmod +x setup.sh && ./setup.sh
```

**For Windows:**
Double-click `setup.bat` or run it from your command line:
```bat
setup.bat
```

### 2. Manual Setup (Alternative)
If you prefer to run the commands manually:
```bash
# Start infrastructure
docker-compose up -d db redis

# Run database migrations
dotnet run --project src/DatabaseUpgrader/TryMeTumble.DatabaseUpgrader.csproj

# Seed the database (make sure API is running)
curl -X POST "http://localhost:5202/api/Websites/seed?count=1000"

# Run application locally
dotnet run --project TryMeTumble.csproj
```

### 3. Access the Application
- **Discover Frontend**: [http://localhost:5202/](http://localhost:5202/)
- **API Base URL**: `http://localhost:5202`
- **Swagger Documentation**: [http://localhost:5202/swagger](http://localhost:5202/swagger)
- **Redis Insights**: Access Redis on `localhost:6381` (Host) or `6379` (Container)

---

## 📖 API Endpoint Reference

### 🌀 Discovery & Websites
* **GET** `/api/Websites/discover` - Returns a random website (Redis cached). Supports optional `?categoryId={id}` for category-specific discovery. **Used by the Frontend Discover page.**
* **POST** `/api/Websites/seed?count=1000` - ⚡ **Mock Data Seeder**: Instantly generates and inserts fake websites.
* **GET** `/api/Websites?page=1&pageSize=10` - Get paginated lists of websites.
* **GET** `/api/Websites/{id}` - Get website details.
* **GET** `/api/Websites/by-category/{categoryId}` - Filter websites.
* **POST** `/api/Websites` - Submit a new website (enforces URL uniqueness).
* **POST** `/api/Websites/{id}/upvote` - Upvote a website.
* **POST** `/api/Websites/{id}/save` - Save a website to your profile.
* **POST** `/api/Websites/report` - Report a website for moderation.
* **GET** `/api/Websites/reports` - Fetch administrative reports for flagged content.

### 🔑 Authentication
* **POST** `/api/Auth/register` - Create a new user.
* **POST** `/api/Auth/login` - Authenticate and retrieve token.
* **GET** `/api/Auth/me` - Get current logged-in user profile.
* **POST** `/api/Auth/logout` - Logout.

### 👥 Users
* **GET** `/api/Users` & `/api/Users/{id}` - Users list and details.
* **GET** `/api/Users/{id}/saved` - Saved websites.
* **GET** `/api/Users/{id}/upvotes` - Upvoted websites.

### 📁 Categories
* **GET** `/api/Categories` - List categories.
* **POST** `/api/Categories` - Add category.

---

## 📊 Database Structure (PostgreSQL)

The application uses **Entity Framework Core** coupled with robust `IEntityTypeConfiguration<T>` mappings to manage the schema.

**Core Tables:**
| Table | Description |
|--------|-------------|
| `Users` | Credentials, emails, and profiles. |
| `Websites` | Primary aggregate URLs. |
| `Categories` | Shared taxonomies/tags for websites. |
| `Tags` | Dynamic tags created by users. |
| `WebsiteTags` | Maps tags to specific websites (Many-to-Many). |
| `Reports` | User-submitted flags for violating websites (One-to-Many). |
| `WebsiteCategories` | Many-to-many associative table. |
| `Upvotes` | Maps users to upvoted websites. |
| `SavedWebsites` | Maps users to their collected private saves. |

---

## 🚀 CI/CD & Deployment

This project uses **GitHub Actions** for Continuous Integration and Continuous Deployment, and is built with **multi-stage Docker builds** to ensure production readiness.

### Docker Multi-Stage Build
The setup provides completely separate stages depending on the environment:
- **`build` Target**: Compiles the .NET 10 project to intermediate binaries.
- **`publish` Target**: Generates the optimized release footprint native ready for deployment.
- **`final` Target**: Uses the lean ASP.NET 10 runtime-alpine image for a minimal attack surface.

### Workflows
- **Test**: Runs on Push/PR to `main`, `staging`, and `develop` branches.
  - Sets up .NET 10 environments.
  - Spins up Postgres and Redis services.
  - Executes **xUnit** tests.

---

## 🛠 Tech Stack

**Core Infrastructure**
- **.NET 10**: The latest version of the high-performance Microsoft framework.
- **C# 12**: Utilizing the newest language features.
- **xUnit & Moq**: Rigorous behavior and unit testing frameworks.
- **PostgreSQL**: Robust relational database for persistent storage.
- **Redis**: In-memory data store for caching and queues.
- **Docker Compose**: Orchestration of the entire stack.

**Libraries & Bundles**
- **Entity Framework Core**: Database interactions and entity management.
- **Swashbuckle**: Automatic OpenAPI / Swagger documentation generation.

---

## 🔨 Development Commands

Here are some common commands you might need during development.

**Database Migrations (EF Core)**
```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations to the database
dotnet ef database update
```

**Check Container Status**
```bash
docker-compose ps
```

**View Logs**
```bash
docker-compose logs -f
```

---

## 📁 Project Structure

```text
.
├── src/
│   ├── API/                  # Controllers, Entry Point, Workers
│   ├── Application/          # DTOs, Mappers, Services, Interfaces
│   ├── Domain/               # Entities, Aggregate Roots, Repository Interfaces
│   ├── Infrastructure/       # DataContext, Configuration, UnitOfWork, Auth
├── tests/
│   └── TryMeTumble.UnitTests/# xUnit tests, Moq dependencies & unit coverage
├── scripts/                  # CI/CD Deployment Scripts
├── TryMeTumble.slnx          # Solution file
├── docker-compose.yml        # Docker services configuration
├── Dockerfile                # .NET multi-stage image definition
└── README.md                 # Project documentation
```
