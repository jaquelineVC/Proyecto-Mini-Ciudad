using MiniCiudad.Domain.Interfaces;
using MiniCiudad.Infrastructure.Algorithms;
using MiniCiudad.Infrastructure.Graph;
using Microsoft.Extensions.DependencyInjection;

namespace MiniCiudad.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IGraphRepository, CityGraphRepository>();
        services.AddSingleton<IRouteCalculator, AStarRouteCalculator>();

        return services;
    }
}