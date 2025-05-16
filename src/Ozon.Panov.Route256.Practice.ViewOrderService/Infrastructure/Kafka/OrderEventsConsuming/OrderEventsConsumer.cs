using Confluent.Kafka;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.Orders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Application.ViewOrders;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.Configuration;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.Serializers;
using Ozon.Route256.OrderService.Proto.Messages;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Kafka.OrderEventsConsuming;

internal sealed class OrderEventsConsumer : BackgroundService
{
    private readonly ILogger<OrderEventsConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topic;
    private readonly TimeSpan _timeoutForRetry;
    private readonly ConsumerConfig _consumerConfig;

    public OrderEventsConsumer(
        IServiceProvider serviceProvider,
        KafkaSettings kafkaSettings,
        ConsumerSettings consumerSettings,
        ILogger<OrderEventsConsumer> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _topic = consumerSettings.Topic;
        _timeoutForRetry = TimeSpan.FromSeconds(kafkaSettings.TimeoutForRetryInSeconds);
        _consumerConfig = new ConsumerConfig
        {
            GroupId = kafkaSettings.GroupId,
            BootstrapServers = kafkaSettings.BootstrapServers,
            EnableAutoCommit = consumerSettings.AutoCommit
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var consumer = new ConsumerBuilder<Ignore, OrderOutputEventMessage>(_consumerConfig)
            .SetValueDeserializer(new ProtoKafkaSerializer<OrderOutputEventMessage>())
            .Build();

        consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Process(consumer, stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task Process(
        IConsumer<Ignore, OrderOutputEventMessage> consumer,
        CancellationToken stoppingToken)
    {
        try
        {
            if (consumer.Consume(stoppingToken) is not { } consumeResult)
            {
                return;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();

            var orderEventsProcessor = scope.ServiceProvider.GetRequiredService<IOrderEventsProcessor>();

            await orderEventsProcessor.ProcessOrderEventMessage(
                consumeResult.Message.Value,
                stoppingToken);

            consumer.Commit(consumeResult);
        }
        catch (ConsumeException e)
        {
            _logger.LogError(e, "Consume error: {Reason}", e.Error.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            await Task.Delay(_timeoutForRetry, stoppingToken);
        }
    }
}
