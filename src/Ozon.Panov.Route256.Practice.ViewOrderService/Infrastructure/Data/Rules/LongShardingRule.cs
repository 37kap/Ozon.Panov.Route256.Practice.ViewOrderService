using Murmur;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;

public class LongShardingRule(int bucketsCount)
    : IShardingRule<long>
{
    public int GetBucketId(
        long shardKey)
    {
        var shardKeyHashCode = GetShardKeyHashCode(shardKey);

        return Math.Abs(shardKeyHashCode) % bucketsCount;
    }

    private static int GetShardKeyHashCode(
        long shardKey)
    {
        var bytes = BitConverter.GetBytes(shardKey);
        var murmur = MurmurHash.Create32();
        var hash = murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}