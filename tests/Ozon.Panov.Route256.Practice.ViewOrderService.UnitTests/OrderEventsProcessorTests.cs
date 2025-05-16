using FluentAssertions;
using Moq;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.OrderEventsConsuming;
using Ozon.Route256.OrderService.Proto.Messages;
using Ozon.Route256.OrderService.Proto.OrderGrpc;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.UnitTests;

public class OrderEventsProcessorTests
{
    private readonly Mock<IOrdersProvider> _ordersProviderMock;
    private readonly Mock<IViewOrderRepository> _viewOrderRepositoryMock;
    private readonly OrderEventsProcessor _processor;

    public OrderEventsProcessorTests()
    {
        _ordersProviderMock = new Mock<IOrdersProvider>();
        _viewOrderRepositoryMock = new Mock<IViewOrderRepository>();
        _processor = new OrderEventsProcessor(
            _viewOrderRepositoryMock.Object,
            _ordersProviderMock.Object);
    }

    [Fact]
    public async Task When_order_does_not_exist_it_inserts_to_database()
    {
        // Arrange
        var message = new OrderOutputEventMessage
        {
            OrderId = 1,
            EventType = OutputEventType.Created
        };

        var orderFromProvider = new OrderEntity()
        {
            OrderId = 1,
            RegionId = 100,
            Status = (int)OrderStatus.New,
            CustomerId = 200,
            Comment = "Comment",
            CreatedAt = DateTime.UtcNow
        };

        _ordersProviderMock
            .Setup(x => x.Find(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderFromProvider);

        _viewOrderRepositoryMock
            .Setup(x => x.Get(It.IsAny<ViewOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _processor.ProcessOrderEventMessage(message, CancellationToken.None);

        // Assert
        _viewOrderRepositoryMock.Verify(
            x => x.Insert(It.Is<OrderEntity>(
                o => o.OrderId == orderFromProvider.OrderId &&
                o.RegionId == orderFromProvider.RegionId &&
                o.Status == orderFromProvider.Status &&
                o.CustomerId == orderFromProvider.CustomerId &&
                o.Comment == orderFromProvider.Comment &&
                o.CreatedAt == orderFromProvider.CreatedAt),
                It.IsAny<CancellationToken>()), Times.Once);

        _viewOrderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task When_order_exists_it_updates_in_database()
    {
        // Arrange
        var message = new OrderOutputEventMessage
        {
            OrderId = 1,
            EventType = OutputEventType.Updated
        };

        var orderFromProvider = new OrderEntity()
        {
            OrderId = 1,
            RegionId = 100,
            Status = (int)OrderStatus.Delivered,
            CustomerId = 200,
            Comment = "Result order comment",
            CreatedAt = DateTime.UtcNow
        };

        var existingOrder = new OrderEntity
        {
            OrderId = 1,
            RegionId = 50,
            Status = (int)OrderStatus.New,
            CustomerId = 150,
            Comment = "First order comment",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _ordersProviderMock
            .Setup(x => x.Find(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderFromProvider);

        _viewOrderRepositoryMock
            .Setup(x => x.Get(It.IsAny<ViewOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([existingOrder]);

        // Act
        await _processor.ProcessOrderEventMessage(message, CancellationToken.None);

        // Assert
        _viewOrderRepositoryMock.Verify(x => x.Update(It.Is<OrderEntity>(o =>
            o.OrderId == orderFromProvider.OrderId &&
            o.RegionId == orderFromProvider.RegionId &&
            o.Status == orderFromProvider.Status &&
            o.CustomerId == orderFromProvider.CustomerId &&
            o.Comment == orderFromProvider.Comment &&
            o.CreatedAt == orderFromProvider.CreatedAt),
            It.IsAny<CancellationToken>()), Times.Once);

        _viewOrderRepositoryMock.Verify(
            x => x.Insert(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task When_order_not_found_in_provider_throws_exception()
    {
        // Arrange
        var message = new OrderOutputEventMessage
        {
            OrderId = 1,
            EventType = OutputEventType.Created
        };

        _ordersProviderMock
            .Setup(x => x.Find(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderEntity?)null);

        // Act
        Func<Task> act = async ()
            => await _processor.ProcessOrderEventMessage(message, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OrderNotFoundInProviderException>()
            .WithMessage("Order 1 does not exist in provider.");
    }

    [Fact]
    public async Task When_throws_exception_it_is_propagated()
    {
        // Arrange
        var message = new OrderOutputEventMessage
        {
            OrderId = 1,
            EventType = OutputEventType.Created
        };

        var orderFromProvider = new OrderEntity()
        {
            OrderId = 1,
            RegionId = 100,
            Status = (int)OrderStatus.New,
            CustomerId = 200,
            Comment = "Comment",
            CreatedAt = DateTime.UtcNow
        };

        _ordersProviderMock
            .Setup(x => x.Find(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderFromProvider);

        _viewOrderRepositoryMock
            .Setup(x => x.Get(It.IsAny<ViewOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async ()
            => await _processor.ProcessOrderEventMessage(message, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");
    }
}
