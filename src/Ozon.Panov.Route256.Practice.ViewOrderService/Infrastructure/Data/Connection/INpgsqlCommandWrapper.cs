using Npgsql;
using System.Data;
using System.Data.Common;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;

public interface INpgsqlCommandWrapper : IDisposable
{
    string CommandText { get; set; }
    int CommandTimeout { get; set; }
    UpdateRowSource UpdatedRowSource { get; set; }
    CommandType CommandType { get; set; }
    NpgsqlConnection? Connection { get; set; }
    DbParameterCollection Parameters { get; }
    NpgsqlTransaction? Transaction { get; set; }
    bool DesignTimeVisible { get; set; }

    Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);
    void Cancel();
    int ExecuteNonQuery();
    object? ExecuteScalar();
    void Prepare();
    DbParameter CreateParameter();
    DbDataReader ExecuteReader(CommandBehavior behavior);
    ValueTask DisposeAsync();
}
