using System.Diagnostics.CodeAnalysis;
using Eventuous;
using Eventuous.EventStore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.EventStore;

namespace Shop.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ConfigureServices
{

    /// <summary>
    /// Adds the infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IMiraEventStore, MiraEventStoreEventuous>()
            .AddSingleton<IEventStore, EsdbEventStore>()
            .AddEventStoreClient("esdb://localhost:2113?tls=false")
            //.AddScoped<IMiraEventStore, MiraEventStoreDbRepository>() // моя реализация
            .AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }


    /// <summary>
    /// Adds the write-only repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddWriteOnlyRepositories(this IServiceCollection services) =>
        services;
    //.AddSingleton<IMiraSnapshotRepository, MiraSnapshotRepository>();
}