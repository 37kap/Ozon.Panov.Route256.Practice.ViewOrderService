namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public interface IShardConnectionFactory
{
    string GetConnectionString(int bucketId);

    IEnumerable<int> GetAllBuckets();
    INpgsqlConnectionWrapper GetConnection(string connectionString);
}
