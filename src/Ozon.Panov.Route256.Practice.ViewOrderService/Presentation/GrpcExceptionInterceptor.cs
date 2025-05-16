using Grpc.Core;
using Grpc.Core.Interceptors;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Presentation;

public sealed class GrpcExceptionInterceptor(
    ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (OrderNotFoundInProviderException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
        catch (OrderAlreadyExistsException exception)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unexpected exception was thrown");
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }
}
