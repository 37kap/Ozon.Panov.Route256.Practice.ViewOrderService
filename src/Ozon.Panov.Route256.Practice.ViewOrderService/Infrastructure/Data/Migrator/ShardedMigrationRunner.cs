using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

public sealed class ShardedMigrationRunner
{
    private static int _bucketsCount = 0;

    public static void MigrateUp(IReadOnlyList<DbEndpoint> endpoints)
    {
        _bucketsCount = endpoints
            .SelectMany(x => x.Buckets)
            .Count();

        foreach (var endpoint in endpoints)
        {
            foreach (var bucketId in endpoint.Buckets)
            {
                var serviceProvider = CreateServices(endpoint, bucketId);
                using var scope = serviceProvider.CreateScope();

                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
    }

    private static IServiceProvider CreateServices(DbEndpoint endpoint, int bucketId)
    {
        var connectionString = GetConnectionString(endpoint);

        var services = new ServiceCollection();
        var provider = services
            .AddScoped<IBucketMigrationContext>(_ => new BucketMigrationContext(endpoint, bucketId))
            .AddSingleton<IConventionSet>(new DefaultConventionSet(null, null))
            .AddSingleton<IShardingRule<long>>(new LongShardingRule(_bucketsCount))
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithRunnerConventions(new MigrationRunnerConventions())
                .WithMigrationsIn(typeof(Program).Assembly)
                .ScanIn(typeof(ShardVersionTableMetaData).Assembly).For.VersionTableMetaData()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();

        return provider;
    }

    private static string GetConnectionString(DbEndpoint endpoint)
    {
        return endpoint.ConnectionString;
    }
}