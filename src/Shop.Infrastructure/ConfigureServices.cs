using System.Diagnostics.CodeAnalysis;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.EventStore;
using Shop.Infrastructure.Data.Repositories;

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
            .AddScoped<IMiraEventStore, MiraEventStoreDbRepository>() // твоя реализация
            .AddScoped<WriteDbContext>()
            .AddScoped<EventStoreDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }


    /// <summary>
    /// Adds the write-only repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddWriteOnlyRepositories(this IServiceCollection services) =>
         services
            .AddScoped<IEventStoreRepository, EventStoreRepository>()
            .AddSingleton<IMiraSnapshotRepository, MiraSnapshotRepository>()
            .AddScoped<ICustomerWriteOnlyRepository, CustomerWriteOnlyRepository>();
}