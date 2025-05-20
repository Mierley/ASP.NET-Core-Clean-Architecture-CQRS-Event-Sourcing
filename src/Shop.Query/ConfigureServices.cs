using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Shop.Query.Abstractions;
using Shop.Query.Data.Mappings;
using Shop.Query.Data.Repositories;
using Shop.Query.Data.Repositories.Abstractions;

namespace Shop.Query;

[ExcludeFromCodeCoverage]
public static class ConfigureServices
{
    /// <summary>
    /// Adds query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(IQueryMarker));
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
            .AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(assembly))))
            .AddValidatorsFromAssembly(assembly);
    }

    /// <summary>
    /// Adds read-only repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddReadOnlyRepositories(this IServiceCollection services) =>
        services.AddScoped<IMiraCustomerReadOnlyRepository, MiraCustomerReadOnlyRepository>();


}