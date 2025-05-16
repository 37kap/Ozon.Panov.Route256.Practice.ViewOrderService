namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

public sealed record DbEndpoint
{
    public required string ConnectionString { get; init; }

    public required int[] Buckets { get; init; }
}