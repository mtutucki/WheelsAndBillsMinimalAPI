using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WheelsAndBills.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(basePath, "WheelsAndBillsAPI", "WheelsAndBills.API");
            if (!File.Exists(Path.Combine(apiPath, "appsettings.json")))
            {
                apiPath = basePath;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("Default") ??
                "Server=localhost;Database=WheelsAndBillsAPI;Trusted_Connection=True;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly("WheelsAndBills.API"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
