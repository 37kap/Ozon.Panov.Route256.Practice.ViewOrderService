using Google.Protobuf.WellKnownTypes;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;
using Ozon.Panov.Route256.Practice.ViewOrderService.ViewOrderServiceGrpc;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application;

public static class OrdersMapper
{
    public static OrderEntity ToEntity(this V1UpdateOrderRequest request) =>
        new()
        {
            OrderId = request.OrderId,
            RegionId = request.RegionId,
            Status = request.Status,
            CustomerId = request.CustomerId,
            Comment = request.Comment,
            CreatedAt = request.CreatedAt.ToDateTimeOffset()
        };

    public static OrderEntity ToEntity(this V1AddOrderRequest request) =>
        new()
        {
            OrderId = request.OrderId,
            RegionId = request.RegionId,
            Status = request.Status,
            CustomerId = request.CustomerId,
            Comment = request.Comment,
            CreatedAt = request.CreatedAt.ToDateTimeOffset()
        };

    public static V1QueryOrderResponse.Types.Order ToGrpcQueryOrderResponse(this OrderEntity order) =>
        new()
        {
            OrderId = order.OrderId,
            RegionId = order.RegionId,
            Status = order.Status,
            CustomerId = order.CustomerId,
            Comment = order.Comment,
            CreatedAt = order.CreatedAt.ToTimestamp()
        };

    public static ViewOrdersQuery ToQuery(this V1QueryOrderRequest request) =>
        new()
        {
            OrderIds = [.. request.OrderIds],
            RegionIds = [.. request.RegionIds],
            CustomerIds = [.. request.CustomerIds],
            Limit = request.Limit,
            Offset = request.Offset
        };
}
