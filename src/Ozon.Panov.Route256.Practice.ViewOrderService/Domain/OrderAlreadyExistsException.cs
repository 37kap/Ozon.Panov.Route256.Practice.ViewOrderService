namespace Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

internal sealed class OrderAlreadyExistsException(long id)
    : Exception($"Order with order_id {id} already exists.");