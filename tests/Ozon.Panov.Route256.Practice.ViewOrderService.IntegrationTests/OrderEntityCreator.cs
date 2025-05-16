using AutoBogus;
using Bogus;
using FluentAssertions.Common;
using Ozon.Panov.Route256.Practice.ViewOrderService.Domain;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.IntegrationTests;

public static class OrderEntityCreator
{
    private static readonly Faker<OrderEntity> Faker = new AutoFaker<OrderEntity>()
        .RuleFor(x => x.OrderId, f => f.Random.Long(10000))
        .RuleFor(x => x.RegionId, f => f.Random.Byte(1))
        .RuleFor(x => x.Status, f => f.Random.Byte(0, 3))
        .RuleFor(x => x.CustomerId, f => f.Random.Long(10000))
        .RuleFor(x => x.Comment, f => f.Lorem.Sentence())
        .RuleFor(x => x.CreatedAt, f => f.Date.Past().Date.ToDateTimeOffset());

    public static OrderEntity Generate()
        => Faker.Generate();
}