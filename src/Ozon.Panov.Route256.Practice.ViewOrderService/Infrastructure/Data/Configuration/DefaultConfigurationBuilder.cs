namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

public static class DefaultConfigurationBuilder
{
    public static ShardConfiguration Build(
        IEnumerable<string> shardConnectionStrings,
        int bucketsPerShard)
    {
        int bucketsCount = shardConnectionStrings.Count() *
                           bucketsPerShard;

        var buckets = Enumerable
            .Range(0, bucketsCount)
            .ToArray();

        var endpoints = new List<DbEndpoint>();
        var index = 0;

        foreach (string connectionString in shardConnectionStrings)
        {
            endpoints.Add(new DbEndpoint
            {
                ConnectionString = connectionString,
                Buckets = [.. buckets
                    .Skip(index * bucketsPerShard)
                    .Take(bucketsPerShard)]
            });
            index++;
        }

        return new ShardConfiguration
        {
            BucketsCount = bucketsCount,
            Endpoints = endpoints
        };
    }
}