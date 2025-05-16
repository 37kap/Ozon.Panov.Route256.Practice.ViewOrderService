namespace Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

public sealed record OrderEntity
{
    public required long OrderId { get; init; }
    public required long RegionId { get; init; }
    public required int Status { get; init; }
    public required long CustomerId { get; init; }
    public string? Comment { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}