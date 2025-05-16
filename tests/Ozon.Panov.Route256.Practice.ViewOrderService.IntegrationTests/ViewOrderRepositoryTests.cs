using FluentAssertions;
using Moq;
using Npgsql;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Repositories;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;
using Ozon.Panov.Route256.Practice.ViewOrderService.IntegrationTests.SpyClasses;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.IntegrationTests;
public class ViewOrderRepositoryTests
{
    public SpyNpgsqlConnectionWrapper SpyConnection { get; }
    public Mock<IShardConnectionFactory> ConnectionFactoryMock { get; }
    public Mock<IShardingRule<long>> ShardingRuleMock { get; }
    public int BucketId { get; }
    public ViewOrderRepository Repository { get; }

    public ViewOrderRepositoryTests()
    {
        var npgConnection = new NpgsqlConnection();
        SpyConnection = new SpyNpgsqlConnectionWrapper(npgConnection);

        ConnectionFactoryMock = new Mock<IShardConnectionFactory>();
        ShardingRuleMock = new Mock<IShardingRule<long>>();
        BucketId = new Random().Next(1, 100);

        ShardingRuleMock.Setup(x => x.GetBucketId(It.IsAny<long>())).Returns(BucketId);
        ConnectionFactoryMock
            .Setup(x => x.GetConnectionString(BucketId))
            .Returns("fake_connection_string");

        ConnectionFactoryMock
            .Setup(x => x.GetConnection("fake_connection_string"))
            .Returns(SpyConnection);

        ConnectionFactoryMock
            .Setup(x => x.GetAllBuckets())
            .Returns([BucketId]);

        Repository = new ViewOrderRepository(
            ConnectionFactoryMock.Object,
            ShardingRuleMock.Object);
    }

    [Fact]
    public async Task Insert_order_should_execute_valid_sql_with_correct_parameters()
    {
        // Arrange
        var order = OrderEntityCreator.Generate();

        // Act
        await Repository.Insert(order, CancellationToken.None);

        // Assert
        var executedCommand = SpyConnection.ExecutedCommands.First();

        executedCommand.CommandText.Should()
            .Contain($"insert into bucket_{BucketId}.orders");

        var parameters = executedCommand.SpyParameters.Items;
        parameters.Should().ContainSingle();

        parameters[0].ParameterName.Should().Be("Orders");
        parameters[0].Value.Should().BeEquivalentTo(new[] { order });
    }

    [Fact]
    public async Task Update_order_should_execute_valid_update_sql_with_correct_parameters()
    {
        // Arrange
        var order = OrderEntityCreator.Generate();

        // Act
        await Repository.Update(order, CancellationToken.None);

        // Assert
        var executedCommand = SpyConnection.ExecutedCommands.First();

        executedCommand.CommandText.Should()
            .Contain($"update bucket_{BucketId}.orders");

        var parameters = executedCommand.SpyParameters.Items;
        parameters.Should().ContainSingle();

        parameters[0].ParameterName.Should().Be("Orders");
        parameters[0].Value.Should().BeEquivalentTo(new[] { order });
    }

    [Fact]
    public async Task Get_orders_by_ids_should_execute_valid_select_sql()
    {
        // Arrange
        var orderIds = new[] { 1L, 2L, 3L };
        var query = new ViewOrdersQuery { OrderIds = orderIds };

        // Act
        _ = await Repository.Get(query, CancellationToken.None);

        // Assert
        var executedCommand = SpyConnection.ExecutedCommands.First();

        executedCommand.CommandText.Should()
            .Contain($"from bucket_{BucketId}.orders")
            .And.Contain("order_id = any(@OrderIds)")
            .And.NotContain("region_id = any(@RegionIds)")
            .And.NotContain("customer_id = any(@CustomerIds)");

        var parameters = executedCommand.SpyParameters.Items;
        parameters.Should().ContainSingle();

        parameters[0].ParameterName.Should().Be("OrderIds");
        parameters[0].Value.Should().BeEquivalentTo(orderIds);
    }

    [Fact]
    public async Task Get_orders_with_multiple_filters_should_combine_conditions_in_sql()
    {
        // Arrange
        var query = new ViewOrdersQuery
        {
            OrderIds = [1L, 2L],
            RegionIds = [10L],
            CustomerIds = [100L],
            Limit = 50,
            Offset = 10
        };

        // Act
        _ = await Repository.Get(query, CancellationToken.None);

        // Assert
        var executedCommand = SpyConnection.ExecutedCommands.First();

        executedCommand.CommandText.Should()
            .Contain("order_id = any(@OrderIds)")
            .And.Contain("region_id = any(@RegionIds)")
            .And.Contain("customer_id = any(@CustomerIds)");

        var parameters = executedCommand.SpyParameters.Items;
        parameters.Should().HaveCount(3);

        parameters.Should().Contain(p => p.ParameterName == "OrderIds");
        parameters.Should().Contain(p => p.ParameterName == "RegionIds");
        parameters.Should().Contain(p => p.ParameterName == "CustomerIds");
    }

    [Fact]
    public async Task Get_orders_with_empty_query_should_throw_argument_exception()
    {
        // Arrange
        var query = new ViewOrdersQuery();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Repository.Get(query, CancellationToken.None));
    }
}