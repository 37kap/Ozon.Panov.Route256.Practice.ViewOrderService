namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;

public static class ShardsHelper
{
    public const string BucketPlaceholder = "__bucket__";
    public static string GetSchemaName(int bucketId) => $"bucket_{bucketId}";
}