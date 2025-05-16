using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

public abstract class ShardSqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        var currentSchema = context.ServiceProvider
            .GetRequiredService<IBucketMigrationContext>()
            .CurrentDbSchema;

        var sqlStatement = GetUpSql(context.ServiceProvider)
            .Replace(ShardsHelper.BucketPlaceholder,
                currentSchema);

        if (!context.QuerySchema.SchemaExists(currentSchema))
        {
            context.Expressions.Add(
                new ExecuteSqlStatementExpression
                {
                    SqlStatement = $"create schema {currentSchema};"
                });
        }

        context.Expressions.Add(
            new ExecuteSqlStatementExpression
            {
                SqlStatement = $"SET search_path TO {currentSchema};"
            });

        context.Expressions.Add(
            new ExecuteSqlStatementExpression
            {
                SqlStatement = sqlStatement
            });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        var currentSchema = context.ServiceProvider
            .GetRequiredService<IBucketMigrationContext>()
            .CurrentDbSchema;

        var sqlStatement = GetDownSql(context.ServiceProvider)
            .Replace(ShardsHelper.BucketPlaceholder,
                currentSchema);

        context.Expressions.Add(
            new ExecuteSqlStatementExpression
            {
                SqlStatement = $"SET search_path TO {currentSchema};"
            });

        context.Expressions.Add(
            new ExecuteSqlStatementExpression
            {
                SqlStatement = sqlStatement
            });
    }

    protected abstract string GetUpSql(IServiceProvider services);
    protected abstract string GetDownSql(IServiceProvider services);
    string IMigration.ConnectionString => throw new NotSupportedException();
}