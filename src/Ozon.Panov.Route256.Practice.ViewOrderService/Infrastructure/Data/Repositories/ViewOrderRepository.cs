using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Repositories;

public sealed class ViewOrderRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule) :
    BaseShardRepository<long>(connectionFactory, shardingRule),
    IViewOrderRepository
{
    public async Task Insert(
        OrderEntity order,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
                           insert into __bucket__.orders (
                               order_id, 
                               region_id, 
                               status, 
                               customer_id, 
                               comment, 
                               created_at
                            )
                            select order_id, region_id, status, 
                                customer_id, comment, created_at
                            from unnest(@Orders);
                           """;

        var cmd = new CommandDefinition(
            sql,
            new
            {
                Orders = new[] { order }
            },
        commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: cancellationToken);

        await using var connection =
            await GetOpenedConnectionByShardKey(
                order.OrderId,
                cancellationToken);

        try
        {
            await connection.ExecuteAsync(cmd);
        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new OrderAlreadyExistsException(order.OrderId);
        }
    }

    public async Task Update(
        OrderEntity order,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           update __bucket__.orders
                           set region_id = o.region_id,
                               status = o.status,
                               customer_id = o.customer_id,
                               comment = o.comment,
                               created_at = o.created_at
                           from unnest(@Orders) as o
                           where orders.order_id = o.order_id;
                           """;

        var cmd = new CommandDefinition(
            sql,
            new
            {
                Orders = new[] { order }
            },
        commandTimeout: DefaultTimeoutInSeconds,
        cancellationToken: cancellationToken);

        await using var connection = await GetOpenedConnectionByShardKey(
            order.OrderId,
            cancellationToken);

        await connection.ExecuteAsync(cmd);
    }

    public async Task<IReadOnlyList<OrderEntity>> Get(
        ViewOrdersQuery query,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        static bool IsNullOrEmpty<T>(IEnumerable<T>? enumerable) =>
            enumerable is null || !enumerable.Any();

        if (IsNullOrEmpty(query.OrderIds) &&
            IsNullOrEmpty(query.RegionIds) &&
            IsNullOrEmpty(query.CustomerIds) &&
            query.Limit == 0)
        {
            throw new ArgumentException(null, nameof(query));
        }

        var sql = """
                  select order_id, region_id, status, 
                    customer_id, comment, created_at
                  from __bucket__.orders
                  where true
                  """;

        var parameters = new DynamicParameters();

        if (!IsNullOrEmpty(query.OrderIds))
        {
            sql += "\nand order_id = any(@OrderIds)";
            parameters.Add("OrderIds", query.OrderIds);
        }

        if (!IsNullOrEmpty(query.RegionIds))
        {
            sql += "\nand region_id = any(@RegionIds)";
            parameters.Add("RegionIds", query.RegionIds);
        }

        if (!IsNullOrEmpty(query.CustomerIds))
        {
            sql += "\nand customer_id = any(@CustomerIds)";
            parameters.Add("CustomerIds", query.CustomerIds);
        }
        sql += ';';

        var cmd = new CommandDefinition(
            sql,
            parameters: parameters,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: cancellationToken);

        IEnumerable<OrderEntity> ordersResult =
            await GetOrdersFromAllBuckets(cmd, cancellationToken);

        ordersResult = ordersResult.OrderBy(order => order.OrderId);
        if (query.Offset > 0)
        {
            ordersResult = ordersResult.Skip(query.Offset);
        }

        if (query.Limit > 0)
        {
            ordersResult = ordersResult.Take(query.Limit);
        }

        return [.. ordersResult];
    }

    private async Task<IReadOnlyList<OrderEntity>> GetOrdersFromAllBuckets(
        CommandDefinition cmd,
        CancellationToken cancellationToken)
    {
        var result = new List<OrderEntity>();

        foreach (var bucket in AllBuckets)
        {
            await using var connection = await GetOpenedConnectionByBucket(
                bucket,
                cancellationToken);

            var ordersInBucket = await connection.QueryAsync<OrderEntity>(cmd);

            result.AddRange(ordersInBucket);
        }

        return result;
    }
}