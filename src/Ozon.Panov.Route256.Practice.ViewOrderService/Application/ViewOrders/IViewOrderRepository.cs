using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;

public interface IViewOrderRepository
{
    Task Insert(
        OrderEntity order,
        CancellationToken cancellationToken);

    Task Update(
        OrderEntity order,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderEntity>> Get(
        ViewOrdersQuery query,
        CancellationToken cancellationToken);
}