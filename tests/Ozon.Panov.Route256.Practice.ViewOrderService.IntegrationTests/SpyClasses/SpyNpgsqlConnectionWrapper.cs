using Npgsql;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.IntegrationTests.SpyClasses;

public class SpyNpgsqlConnectionWrapper(
    NpgsqlConnection npgsqlConnection)
    : INpgsqlConnectionWrapper, IDisposable, IAsyncDisposable
{
    public List<SpyNpgsqlCommandWrapper> ExecutedCommands { get; } = [];
    public ConnectionState State { get; private set; }
    public string ConnectionString { get => ""; set { } }

    public string Database => throw new NotImplementedException();

    public string DataSource => throw new NotImplementedException();

    public string ServerVersion => throw new NotImplementedException();

    public INpgsqlCommandWrapper CreateCommand()
    {
        var cmd = new SpyNpgsqlCommandWrapper
        {
            Connection = npgsqlConnection
        };
        ExecutedCommands.Add(cmd);
        return cmd;
    }

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        State = ConnectionState.Open;
        return Task.CompletedTask;
    }

    public void Dispose() { }

    public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        throw new NotImplementedException();
    }

    public void ChangeDatabase(string databaseName)
    {
        throw new NotImplementedException();
    }

    public void Open()
    {
    }

    public void Close()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
