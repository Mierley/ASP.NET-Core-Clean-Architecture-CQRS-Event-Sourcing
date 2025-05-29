using System.Diagnostics.CodeAnalysis;
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
            .AddScoped<IMiraEventStore, MiraEventStoreDbRepository>() // моя реализация
            .AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}