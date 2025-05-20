using System.Diagnostics.CodeAnalysis;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;

namespace Shop.PublicApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServicesCollectionExtensions
{
    private const int DbMaxRetryCount = 3;
    private const int DbCommandTimeout = 30;
    private const string DbMigrationAssemblyName = "Shop.PublicApi";

    public static IServiceCollection AddEventStoreDbClientService(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetOptions<ConnectionOptions>();
        var settings = EventStoreClientSettings.Create(options.EventStoreDbConnection);
        var eventStoreClient = new EventStoreClient(settings);

        return services.AddSingleton(eventStoreClient);
    }
}