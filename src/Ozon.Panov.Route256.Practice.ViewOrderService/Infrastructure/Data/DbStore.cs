using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;

public class DbStore : IDbStore
{
    private readonly Dictionary<long, DbEndpoint> _bucketEndpoint;

    public DbStore(IReadOnlyList<DbEndpoint> dbEndpoints)
    {
        _bucketEndpoint = new Dictionary<long, DbEndpoint>();
        int bucketsCount = 0;

        foreach (var endpoint in dbEndpoints)
        {
            foreach (var bucket in endpoint.Buckets)
            {
                _bucketEndpoint.Add(bucket, endpoint);
                bucketsCount++;
            }
        }

        BucketsCount = bucketsCount;
    }

    public DbEndpoint GetEndpointByBucket(
        int bucketId)
    {
        if (!_bucketEndpoint.TryGetValue(bucketId, out DbEndpoint? bucket))
        {
            throw new ArgumentOutOfRangeException($"There is not bucket {bucketId}");
        }

        return bucket;
    }

    public int BucketsCount { get; private set; }
}
