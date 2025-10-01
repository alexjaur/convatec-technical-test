using Logistics.Application.Clients.ShipmentClient;
using Logistics.Application.Services;
using Logistics.Infrastructure.Clients.ShipmentClient;
using Logistics.Infrastructure.Persistence;
using Logistics.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInternalDependencies(this IServiceCollection serviceCollection, ConfigurationManager configurationManager)
        {
            // Persistence (db context)
            serviceCollection.AddDbContext<LogisticsDbContext>(options =>
                options.UseSqlServer(configurationManager.GetConnectionString("Default"))
            );

            // Services
            serviceCollection
                .AddScoped<ITransporterService, TransporterService>();

            // Clients
            serviceCollection
                .AddScoped<IShipmentClient, FakeStoreClient>();

            // Adding HttpClient for external API calls
            serviceCollection
                .AddHttpClient(nameof(FakeStoreClient), httpClient =>
                {
                    httpClient.BaseAddress = new Uri("https://fakeapi.net");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    //httpClient.DefaultRequestHeaders.Add("api-call", "true");
                });

            return serviceCollection;
        }
    }
}
