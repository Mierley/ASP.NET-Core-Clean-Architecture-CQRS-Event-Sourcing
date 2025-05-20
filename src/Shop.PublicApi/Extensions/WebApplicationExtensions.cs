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

        app.Logger.LogInformation("----- Application is starting....");

        await app.RunAsync();
    }
}