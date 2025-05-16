using Npgsql;
using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public class NpgsqlCommandWrapper(
    NpgsqlCommand command)
    : INpgsqlCommandWrapper,
    IDisposable,
    IAsyncDisposable
{
    public string CommandText
    {
        get => command.CommandText;
        set => command.CommandText = value;
    }

    public int CommandTimeout
    {
        get => command.CommandTimeout;
        set => command.CommandTimeout = value;
    }

    public UpdateRowSource UpdatedRowSource
    {
        get => command.UpdatedRowSource;
        set => command.UpdatedRowSource = value;
    }

    public CommandType CommandType
    {
        get => command.CommandType;
        set => command.CommandType = value;
    }

    public NpgsqlConnection? Connection
    {
        get => command.Connection;
        set => command.Connection = value;
    }

    public DbParameterCollection Parameters
    {
        get => command.Parameters;
    }

    public NpgsqlTransaction? Transaction
    {
        get => command.Transaction;
        set => command.Transaction = value;
    }

    public bool DesignTimeVisible
    {
        get => command.DesignTimeVisible;
        set => command.DesignTimeVisible = value;
    }

    public void Cancel()
    {
        command.Cancel();
    }

    public DbParameter CreateParameter()
    {
        return command.CreateParameter();
    }

    public void Dispose()
    {
        command.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await command.DisposeAsync();
    }

    public int ExecuteNonQuery()
    {
        return command.ExecuteNonQuery();
    }

    public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public DbDataReader ExecuteReader(CommandBehavior behavior)
    {
        return command.ExecuteReader(behavior);
    }

    public object? ExecuteScalar()
    {
        return command.ExecuteScalar();
    }

    public void Prepare()
    {
        command.Prepare();
    }
}
