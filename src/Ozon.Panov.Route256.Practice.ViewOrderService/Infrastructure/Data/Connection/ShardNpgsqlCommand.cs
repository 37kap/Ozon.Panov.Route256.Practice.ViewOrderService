using Npgsql;
using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public class ShardNpgsqlCommand(
    INpgsqlCommandWrapper NpgsqlCommand, int BucketId)
    : DbCommand
{
    public override string CommandText
    {
        get => NpgsqlCommand.CommandText;
        set
        {
            var command = value.Replace(
                ShardsHelper.BucketPlaceholder,
                ShardsHelper.GetSchemaName(BucketId));
            NpgsqlCommand.CommandText = command;
        }
    }

    public override void Cancel() => NpgsqlCommand.Cancel();

    public override int ExecuteNonQuery() => NpgsqlCommand.ExecuteNonQuery();

    public override object? ExecuteScalar() => NpgsqlCommand.ExecuteScalar();

    public override void Prepare() => NpgsqlCommand.Prepare();

    public override int CommandTimeout
    {
        get => NpgsqlCommand.CommandTimeout;
        set => NpgsqlCommand.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
        get => NpgsqlCommand.CommandType;
        set => NpgsqlCommand.CommandType = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => NpgsqlCommand.UpdatedRowSource;
        set => NpgsqlCommand.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
        get => NpgsqlCommand.Connection;
        set => NpgsqlCommand.Connection = value as NpgsqlConnection;
    }

    protected override DbParameterCollection DbParameterCollection =>
        NpgsqlCommand.Parameters;

    protected override DbTransaction? DbTransaction
    {
        get => NpgsqlCommand.Transaction;
        set => NpgsqlCommand.Transaction = value as NpgsqlTransaction;
    }

    public override bool DesignTimeVisible
    {
        get => NpgsqlCommand.DesignTimeVisible;
        set => NpgsqlCommand.DesignTimeVisible = value;
    }

    protected override DbParameter CreateDbParameter() =>
        NpgsqlCommand.CreateParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
        NpgsqlCommand.ExecuteReader(behavior);

    public override ValueTask DisposeAsync() =>
        NpgsqlCommand.DisposeAsync();
}