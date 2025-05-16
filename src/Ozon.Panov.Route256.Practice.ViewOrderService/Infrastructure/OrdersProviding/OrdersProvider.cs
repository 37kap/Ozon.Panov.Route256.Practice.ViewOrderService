using Grpc.Core;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;
using Ozon.Route256.OrderService.Proto.OrderGrpc;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.OrdersProviding;

public sealed class OrdersProvider(OrderGrpc.OrderGrpcClient orderGrpcClient) : IOrdersProvider
{
    public async Task<OrderEntity?> Find(
        long orderId,
        CancellationToken cancellationToken)
    {
        var request = new V1QueryOrdersRequest
        {
            OrderIds = { orderId }
        };

        using var ordersStream = orderGrpcClient
            .V1QueryOrders(
                request,
                cancellationToken: cancellationToken);

        while (await ordersStream.ResponseStream.MoveNext())
        {
            var ordersResponse = ordersStream.ResponseStream.Current;

            return new OrderEntity()
            {
                OrderId = ordersResponse.OrderId,
                RegionId = ordersResponse.Region.Id,
                Status = (int)ordersResponse.Status,
                CustomerId = ordersResponse.CustomerId,
                Comment = ordersResponse.Comment,
                CreatedAt = ordersResponse.CreatedAt.ToDateTime()
            };
        }

        return null;
    }
}