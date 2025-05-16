using Npgsql;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public class ShardConnectionFactory(IDbStore dbStore) : IShardConnectionFactory
{
    public IEnumerable<int> GetAllBuckets()
    {
        for (int bucketId = 0; bucketId < dbStore.BucketsCount; bucketId++)
        {
            yield return bucketId;
        }
    }

    public string GetConnectionString(int bucketId)
        => dbStore.GetEndpointByBucket(bucketId).ConnectionString;

    public INpgsqlConnectionWrapper GetConnection(string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        MapCompositeTypes(dataSourceBuilder);
        var dataSource = dataSourceBuilder.Build();
        var pgConnection = dataSource.CreateConnection();

        return new NpgsqlConnectionWrapper(pgConnection);
    }

    private static void MapCompositeTypes(NpgsqlDataSourceBuilder builder)
    {
        builder.MapComposite<OrderEntity>("order_v1");
    }
}