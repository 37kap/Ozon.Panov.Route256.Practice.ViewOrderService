using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public class ShardNpgsqlConnection(
    INpgsqlConnectionWrapper NpgsqlConnection, int BucketId)
    : DbConnection
{
    protected override DbCommand CreateDbCommand()
    {
        var command = NpgsqlConnection.CreateCommand();
        return new ShardNpgsqlCommand(command, BucketId);
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        NpgsqlConnection.BeginTransaction(isolationLevel);

    public override void ChangeDatabase(string databaseName) =>
        NpgsqlConnection.ChangeDatabase(databaseName);

    public override void Close() => NpgsqlConnection.Close();

    public override void Open() => NpgsqlConnection.Open();

    public override string ConnectionString
    {
        get => NpgsqlConnection.ConnectionString;
        set => NpgsqlConnection.ConnectionString = value;
    }

    public override string Database => NpgsqlConnection.Database;
    public override ConnectionState State => NpgsqlConnection.State;
    public override string DataSource => NpgsqlConnection.DataSource;
    public override string ServerVersion => NpgsqlConnection.ServerVersion;

    public override ValueTask DisposeAsync() =>
        NpgsqlConnection.DisposeAsync();
}