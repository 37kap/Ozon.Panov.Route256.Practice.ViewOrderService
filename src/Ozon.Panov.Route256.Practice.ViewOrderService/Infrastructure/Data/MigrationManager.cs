using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;

public static class MigrationManager
{
    public static IHost MigrateDatabase(
        this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var config = scope.ServiceProvider.GetRequiredService<ShardConfiguration>();

        try
        {
            ShardedMigrationRunner.MigrateUp(config.Endpoints);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while migrating the database");
            throw;
        }

        return host;
    }
}