using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Connection;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Repositories;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Rules;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.OrderEventsConsuming;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.OrdersProviding;
using Ozon.Panov.Route256.Practice.ViewOrderService.Presentation;
using Ozon.Route256.OrderService.Proto.OrderGrpc;
using System.Reflection;

namespace Ozon.Panov.Route256.Practice.ViewOrderService;

public static class Composer
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services)
    {
        services
            .AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.Interceptors.Add<GrpcExceptionInterceptor>();
            })
            .AddJsonTranscoding();

        return services.AddGrpcSwagger()
            .AddSwaggerGen(c =>
            {
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            })
            .AddGrpcReflection();
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddGrpcClients(configuration)
            .AddShardedDatabase(configuration)
            .AddScoped<IViewOrderRepository, ViewOrderRepository>()
            .AddScoped<IOrdersProvider, OrdersProvider>()
            .AddKafka(configuration);
    }

    private static IServiceCollection AddShardedDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services.AddShardsSettings(configuration)
            .AddSingleton<IDbStore>(s =>
                new DbStore(s.GetRequiredService<ShardConfiguration>().Endpoints))
            .AddSingleton<IShardingRule<long>>(s =>
                new LongShardingRule(s.GetRequiredService<ShardConfiguration>().BucketsCount))
            .AddScoped<IShardConnectionFactory, ShardConnectionFactory>();
    }

    private static IServiceCollection AddShardsSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddSingleton(s =>
        {
            IEnumerable<string> shardConnectionStrings =
            [
                configuration.GetValue<string>("VIEW_ORDER_SERVICE_DB_SHARD1_CONNECTION_STRINGS")!,
                configuration.GetValue<string>("VIEW_ORDER_SERVICE_DB_SHARD2_CONNECTION_STRINGS")!
            ];
            var bucketsPerShard = configuration.GetValue<int>("VIEW_ORDER_SERVICE_DB_BUCKETS_PER_SHARD");

            ValidateChardConfig(shardConnectionStrings, bucketsPerShard);

            return DefaultConfigurationBuilder.Build(shardConnectionStrings, bucketsPerShard);
        });
    }

    private static void ValidateChardConfig(
        IEnumerable<string> shardConnectionStrings,
        int bucketsPerShard)
    {
        if (shardConnectionStrings.Any(s => string.IsNullOrEmpty(s)))
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "ShardConnectionStrings cannot be null or empty."));
        }

        if (bucketsPerShard < 1 || bucketsPerShard > 100)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    detail: "BucketsPerShard must be between 1 and 100."));
        }
    }

    private static IServiceCollection AddGrpcClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddOrderServiceGrpcClient(configuration);
    }

    private static IServiceCollection AddOrderServiceGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var orderServiceUrl = configuration.GetValue<string>("ORDER_SERVICE_URL");

        services.AddGrpcClient<OrderGrpc.OrderGrpcClient>(options =>
            {
                options.Address = new Uri(orderServiceUrl!);
            })
            .ConfigureChannel(options =>
            {
                options.ServiceConfig = new ServiceConfig
                {
                    MethodConfigs = { GetDefaultMethodConfig() }
                };
            });

        return services;
    }

    private static MethodConfig GetDefaultMethodConfig()
    {
        return new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromMilliseconds(10),
                MaxBackoff = TimeSpan.FromMilliseconds(25),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }

    private static IServiceCollection AddKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>()!;
        var bootstrapServers = configuration.GetValue<string>("KAFKA_BROKERS")!;

        kafkaSettings.BootstrapServers = bootstrapServers;
        services.AddConsumers(configuration, kafkaSettings);

        return services
            .AddScoped<IOrderEventsProcessor, OrderEventsProcessor>();
    }

    private static IServiceCollection AddConsumers(
        this IServiceCollection services,
        IConfiguration configuration,
        KafkaSettings kafkaSettings)
    {
        return services
            .AddHostedService(serviceProvider =>
             new OrderEventsConsumer(
                 serviceProvider,
                 kafkaSettings,
                 configuration
                     .GetSection("Kafka:Consumer:OrderEventsConsumer")
                     .Get<ConsumerSettings>()!,
                 serviceProvider.GetRequiredService<ILogger<OrderEventsConsumer>>()));
    }
}
