using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shop.PublicApi.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task RunAppAsync(this WebApplication app)
    {
        await using var serviceScope = app.Services.CreateAsyncScope();

        var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

        app.Logger.LogInformation("----- AutoMapper: mappings are being validated...");

        // Validate the AutoMapper configuration by asserting that the mappings are valid
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        // Compile the AutoMapper mappings for better performance
        mapper.ConfigurationProvider.CompileMappings();

        app.Logger.LogInformation("----- AutoMapper: mappings are valid!");

        app.Logger.LogInformation("----- Databases are being migrated....");

        app.Logger.LogInformation("----- Databases have been successfully migrated!");

        app.Logger.LogInformation("----- Application is starting....");

        await app.RunAsync();
    }
}