using Npgsql;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.IntegrationTests.SpyClasses;

public class SpyNpgsqlCommandWrapper : INpgsqlCommandWrapper
{
    public string CommandText { get; set; } = string.Empty;
    public int CommandTimeout { get; set; }
    public UpdateRowSource UpdatedRowSource { get; set; }
    public CommandType CommandType { get; set; }
    public NpgsqlConnection? Connection { get; set; }

    private SpyDbParameterCollection _parameters = new SpyDbParameterCollection();
    public DbParameterCollection Parameters => _parameters;
    internal SpyDbParameterCollection SpyParameters => _parameters;

    public NpgsqlTransaction? Transaction { get; set; }
    public bool DesignTimeVisible { get; set; }

    public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }

    public void Dispose() { }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public int ExecuteNonQuery()
    {
        return default;
    }

    public object? ExecuteScalar()
    {
        return default;
    }

    public void Prepare()
    {
    }

    public DbParameter CreateParameter()
    {
        var param = new SpyDbParameter();
        return param;
    }

    public DbDataReader ExecuteReader(CommandBehavior behavior)
    {
        return new SpyDataReader();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    internal class SpyDataReader : DbDataReader
    {
        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => 0;

        public override bool HasRows => throw new NotImplementedException();

        public override bool IsClosed => throw new NotImplementedException();

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }
    }

    internal class SpyDbParameter : DbParameter
    {
        public override DbType DbType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ParameterDirection Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool IsNullable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ParameterName { get; set; }
        public override int Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string SourceColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool SourceColumnNullMapping { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override object? Value { get; set; }

        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }
    }
    internal class SpyDbParameterCollection : DbParameterCollection
    {
        public List<SpyDbParameter> Items = new List<SpyDbParameter>();
        public override int Count => throw new NotImplementedException();

        public override object SyncRoot => throw new NotImplementedException();

        public override int Add(object value)
        {
            if (value is SpyDbParameter param)
            {
                Items.Add(param);
            }
            return Items.Count;
        }

        public override void AddRange(Array values)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string value)
        {
            throw new NotImplementedException();
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public override int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        public override void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public override void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public override void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public override void RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(int index)
        {
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            throw new NotImplementedException();
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            throw new NotImplementedException();
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            throw new NotImplementedException();
        }
    }
}
