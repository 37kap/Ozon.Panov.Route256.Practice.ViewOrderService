using Npgsql;
using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public class NpgsqlConnectionWrapper(
    NpgsqlConnection connection)
    : INpgsqlConnectionWrapper, IDisposable, IAsyncDisposable
{
    public ConnectionState State => connection.State;

    public string ConnectionString
    {
        get => connection.ConnectionString;
        set => connection.ConnectionString = value;
    }

    public string Database { get => connection.Database; }
    public string DataSource { get => connection.DataSource; }
    public string ServerVersion { get => connection.ServerVersion; }

    public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        return connection.BeginTransaction(isolationLevel);
    }

    public void ChangeDatabase(string databaseName)
    {
        connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        connection.Close();
    }

    public INpgsqlCommandWrapper CreateCommand()
    {
        return new NpgsqlCommandWrapper(connection.CreateCommand());
    }

    public void Dispose()
    {
        connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
    }

    public void Open()
    {
        connection.Open();
    }

    public async Task OpenAsync(CancellationToken cancellationToken)
    {
        await connection.OpenAsync(cancellationToken);
    }
}
