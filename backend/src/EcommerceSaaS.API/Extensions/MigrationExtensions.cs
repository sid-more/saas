using EcommerceSaaS.Infrastructure.Persistence;

namespace EcommerceSaaS.API.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Create database if it doesn't exist
                await context.Database.EnsureCreatedAsync();

                // Seed sample data
                await DatabaseSeeder.SeedAsync(context);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error during database migration and seeding");
            }
        }
    }
}
