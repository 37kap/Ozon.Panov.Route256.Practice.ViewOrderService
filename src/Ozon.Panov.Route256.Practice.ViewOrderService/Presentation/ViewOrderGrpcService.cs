using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.ViewOrderServiceGrpc;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Presentation;

public sealed class ViewOrderGrpcService(IViewOrderRepository viewOrderRepository) :
    ViewOrderServiceGrpc.ViewOrderServiceGrpc.ViewOrderServiceGrpcBase
{
    public override async Task<V1AddOrderResponse> V1AddOrder(
        V1AddOrderRequest request,
        ServerCallContext context)
    {
        ValidateAddOrderRequest(request);

        await viewOrderRepository.Insert(
            request.ToEntity(),
            context.CancellationToken);

        return new V1AddOrderResponse();
    }

    public override async Task<V1UpdateOrderResponse> V1UpdateOrder(
        V1UpdateOrderRequest request,
        ServerCallContext context)
    {
        ValidateUpdateOrderRequest(request);

        await viewOrderRepository.Update(
            request.ToEntity(),
            context.CancellationToken);

        return new V1UpdateOrderResponse();
    }

    public override async Task<V1QueryOrderResponse> V1QueryOrder(
        V1QueryOrderRequest request,
        ServerCallContext context)
    {
        ValidateQueryOrderRequest(request);

        var orders = await viewOrderRepository.Get(
            request.ToQuery(),
            context.CancellationToken);

        V1QueryOrderResponse response = new()
        {
            Orders =
            {
                orders.Select(order => order.ToGrpcQueryOrderResponse())
            }
        };
        return response;
    }

    private static void ValidateAddOrderRequest(V1AddOrderRequest request)
    {
        if (request.OrderId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "OrderId must be greater than 0."));
        }

        if (request.RegionId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "RegionId must be greater than 0."));
        }

        if (request.CustomerId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "CustomerId must be greater than 0."));
        }

        if (request.Status < 0 || request.Status > 5)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Status must be between 0 and 5."));
        }

        var comment = request.Comment?.Trim();
        if (comment != null &&
            (comment.Length == 0
            || string.IsNullOrWhiteSpace(comment)))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Comment cannot be empty if provided."));
        }

        if (request.CreatedAt == new Timestamp())
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "CreatedAt must be a valid timestamp."));
        }
    }

    private static void ValidateUpdateOrderRequest(V1UpdateOrderRequest request)
    {
        if (request.OrderId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "OrderId must be greater than 0."));
        }

        if (request.RegionId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "RegionId must be greater than 0."));
        }

        if (request.CustomerId <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "CustomerId must be greater than 0."));
        }

        if (request.Status < 0 || request.Status > 5)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Status must be between 0 and 5."));
        }

        var comment = request.Comment?.Trim();
        if (comment != null &&
            (comment.Length == 0
            || string.IsNullOrWhiteSpace(comment)))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Comment cannot be empty if provided."));
        }

        if (request.CreatedAt == new Timestamp())
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "CreatedAt must be a valid timestamp."));
        }
    }

    private static void ValidateQueryOrderRequest(V1QueryOrderRequest request)
    {
        if (request.OrderIds.Any(id => id <= 0))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "OrderId should be greater than 0."));
        }

        if (request.CustomerIds.Any(id => id <= 0))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "CustomerIds should be greater than 0."));
        }

        if (request.RegionIds.Any(id => id <= 0))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "RegionIds should be greater than 0."));
        }

        if (request.Limit <= 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Limit should be greater than 0."));
        }

        if (request.Offset < 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "Offset should not be less than 0."));
        }
    }
}