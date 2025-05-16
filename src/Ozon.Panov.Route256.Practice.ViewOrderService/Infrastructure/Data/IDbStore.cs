using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;

public interface IDbStore
{
    DbEndpoint GetEndpointByBucket(int bucketId);

    int BucketsCount { get; }
}