using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vab.NetLibrary.Hosting.RestartableServices;

public static class Hosting
{
    public static IServiceCollection AddRestartableService<TImplementation>(this IServiceCollection services)
        where TImplementation : RestartableService
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<IHostedService, TImplementation>(serviceProvider =>
            serviceProvider.GetService<TImplementation>()!);
        return services;
    }

    public static IServiceCollection AddRestartableService<TService, TImplementation>(this IServiceCollection services)
        where TService : class where TImplementation : RestartableService, TService
    {
        services.AddSingleton<TService, TImplementation>();
        services.AddSingleton<IHostedService, TImplementation>(serviceProvider =>
            (TImplementation)serviceProvider.GetService<TService>()!);
        return services;
    }
}