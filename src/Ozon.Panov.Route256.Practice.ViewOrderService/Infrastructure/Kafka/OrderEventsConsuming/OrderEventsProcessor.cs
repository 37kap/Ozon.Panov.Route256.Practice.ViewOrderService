using Ozon.Panov.Route256.Practice.ViewOrderService.Application;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Route256.OrderService.Proto.Messages;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.OrderEventsConsuming;

public sealed class OrderEventsProcessor(
    IViewOrderRepository viewOrderRepository,
    IOrdersProvider ordersProvider)
    : IOrderEventsProcessor
{
    public async Task ProcessOrderEventMessage(
        OrderOutputEventMessage message,
        CancellationToken cancellationToken)
    {
        if (await ordersProvider.Find(
                message.OrderId,
                cancellationToken) is not { } order)
        {
            throw new OrderNotFoundInProviderException(message.OrderId);
        }

        var existingOrders = await viewOrderRepository.Get(
            new ViewOrdersQuery
            {
                OrderIds = [order.OrderId]
            },
            cancellationToken);

        if (existingOrders.Count == 0)
        {
            await viewOrderRepository.Insert(order, cancellationToken);
        }
        else
        {
            await viewOrderRepository.Update(
                existingOrders.Single() with
                {
                    RegionId = order.RegionId,
                    Status = order.Status,
                    CustomerId = order.CustomerId,
                    Comment = order.Comment,
                    CreatedAt = order.CreatedAt
                },
                cancellationToken);
        }
    }
}
