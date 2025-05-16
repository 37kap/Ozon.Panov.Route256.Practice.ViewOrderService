namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;

public sealed record ViewOrdersQuery(
    IReadOnlyList<long>? OrderIds = null,
    IReadOnlyList<long>? RegionIds = null,
    IReadOnlyList<long>? CustomerIds = null,
    int Limit = 0,
    int Offset = 0);
