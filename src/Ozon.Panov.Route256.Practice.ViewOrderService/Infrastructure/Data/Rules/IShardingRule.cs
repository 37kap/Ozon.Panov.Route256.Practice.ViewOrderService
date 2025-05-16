namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(TShardKey shardKey);
}