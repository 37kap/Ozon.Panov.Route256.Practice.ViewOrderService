using FluentMigrator.Runner.VersionTableInfo;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

public class ShardVersionTableMetaData(IBucketMigrationContext context) : IVersionTableMetaData
{
    public bool OwnsSchema => true;

    public string SchemaName => context.CurrentDbSchema;

    public string TableName => "version_info";

    public string ColumnName => "version";

    public string DescriptionColumnName => "description";

    public string UniqueIndexName => "version_unique_idx";

    public string AppliedOnColumnName => "applied_on";

    public bool CreateWithPrimaryKey => false;
}