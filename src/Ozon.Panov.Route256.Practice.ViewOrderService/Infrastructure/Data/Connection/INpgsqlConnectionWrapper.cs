using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public interface INpgsqlConnectionWrapper : IDisposable
{
    Task OpenAsync(CancellationToken cancellationToken);
    INpgsqlCommandWrapper CreateCommand();
    DbTransaction BeginTransaction(IsolationLevel isolationLevel);
    void ChangeDatabase(string databaseName);
    void Open();
    void Close();
    ValueTask DisposeAsync();

    ConnectionState State { get; }
    string ConnectionString { get; set; }
    string Database { get; }
    string DataSource { get; }
    string ServerVersion { get; }
}
