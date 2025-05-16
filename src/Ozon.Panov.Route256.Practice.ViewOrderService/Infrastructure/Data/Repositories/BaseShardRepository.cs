using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;
using System.Data;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Repositories;

public class BaseShardRepository<TShardKey>(
    IShardConnectionFactory connectionFactory,
    IShardingRule<TShardKey> shardingRule)
{
    protected const int DefaultTimeoutInSeconds = 5;

    protected async Task<ShardNpgsqlConnection> GetOpenedConnectionByShardKey(
        TShardKey shardKey,
        CancellationToken cancellationToken)
    {
        var bucketId = shardingRule.GetBucketId(shardKey);
        var connection = GetConnectionByBucketId(bucketId);

        return await BaseShardRepository<TShardKey>.OpenConnectionIfNeeded(
            connection,
            cancellationToken);
    }

    protected async Task<ShardNpgsqlConnection> GetOpenedConnectionByBucket(
        int bucketId,
        CancellationToken cancellationToken)
    {
        var connection = GetConnectionByBucketId(bucketId);

        return await BaseShardRepository<TShardKey>.OpenConnectionIfNeeded(
            connection,
            cancellationToken);
    }

    protected int GetBucketByShardKey(TShardKey shardKey)
        => shardingRule.GetBucketId(shardKey);

    protected IEnumerable<int> AllBuckets
        => connectionFactory.GetAllBuckets();

    private ShardNpgsqlConnection GetConnectionByBucketId(int bucketId)
    {
        var connectionString = connectionFactory.GetConnectionString(bucketId);
        var connection = connectionFactory.GetConnection(connectionString);

        return new ShardNpgsqlConnection(connection, bucketId);
    }

    private static async Task<ShardNpgsqlConnection> OpenConnectionIfNeeded(
        ShardNpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        if (connection.State is ConnectionState.Closed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        return connection;
    }
}