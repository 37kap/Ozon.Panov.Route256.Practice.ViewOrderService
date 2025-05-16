using Ozon.Route256.OrderService.Proto.Messages;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;

public interface IOrderEventsProcessor
{
    Task ProcessOrderEventMessage(
        OrderOutputEventMessage message,
        CancellationToken cancellationToken);
}
