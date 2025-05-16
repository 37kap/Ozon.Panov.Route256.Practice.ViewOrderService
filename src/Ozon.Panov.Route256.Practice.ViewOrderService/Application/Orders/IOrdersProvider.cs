using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;

public interface IOrdersProvider
{
    Task<OrderEntity?> Find(
        long orderId,
        CancellationToken cancellationToken);
}