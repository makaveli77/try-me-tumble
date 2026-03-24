using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace TryMeTumble.DatabaseUpgrader
{
    class Program
    {
        static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddEnvironmentVariables(); // E.g. POSTGRES_USER, POSTGRES_PASSWORD

            var configuration = builder.Build();

            // Default fallback or built from .env/docker-compose
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                var user = configuration["POSTGRES_USER"] ?? "postgres";
                var pass = configuration["POSTGRES_PASSWORD"] ?? "password";
                var db = configuration["POSTGRES_DB"] ?? "try-me-tumble";
                var host = configuration["DB_HOST"] ?? "localhost";
                
                connectionString = $"Host={host};Database={db};Username={user};Password={pass}";
            }

            Console.WriteLine("Applying Database Migrations...");

            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success! Database schema is up to date.");
            Console.ResetColor();
            return 0;
        }
    }
}
