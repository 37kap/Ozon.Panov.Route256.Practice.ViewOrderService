using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data;
using Ozon.Panov.Route256.Practice.ViewOrderService.Presentation;

namespace Ozon.Panov.Route256.Practice.ViewOrderService;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        IConfigurationRoot configuration = builder.Configuration
            .AddEnvironmentVariables("ROUTE256_")
            .Build();

        builder.Services
            .AddInfrastructure(configuration)
            .AddPresentation();

        WebApplication app = builder.Build();

        app.MigrateDatabase();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.MapGrpcService<ViewOrderGrpcService>();
        app.MapGrpcReflectionService();

        app.Run();
    }
}
