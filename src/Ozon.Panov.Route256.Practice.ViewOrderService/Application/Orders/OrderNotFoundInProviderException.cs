namespace Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;

public sealed class OrderNotFoundInProviderException(long orderId) :
    Exception($"Order {orderId} does not exist in provider.");