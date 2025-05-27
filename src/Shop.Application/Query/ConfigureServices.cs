using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Abstractions;

namespace Shop.Application.Query;

[ExcludeFromCodeCoverage]
public static class ConfigureServices
{
    /// <summary>
    /// Adds query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(IApplicationMarker));
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
           .AddValidatorsFromAssembly(assembly);
    }

    /// <summary>
    /// Adds read-only repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddReadOnlyRepositories(this IServiceCollection services) =>
        services.AddScoped<IMiraCustomerReadOnlyRepository, MiraCustomerReadOnlyRepository>();


}