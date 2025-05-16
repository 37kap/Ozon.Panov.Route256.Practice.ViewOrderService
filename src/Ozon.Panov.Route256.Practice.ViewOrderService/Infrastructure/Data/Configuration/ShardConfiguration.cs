namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;

public record ShardConfiguration
{
    public required int BucketsCount { get; init; }

    public required IReadOnlyList<DbEndpoint> Endpoints { get; init; }
}