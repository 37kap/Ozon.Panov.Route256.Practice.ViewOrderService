using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

public interface IBucketMigrationContext
{
    string CurrentDbSchema { get; }

    int CurrentBucketId { get; }

    DbEndpoint CurrentEndpoint { get; }
}