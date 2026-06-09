using MiniCiudad.Application.DTOs;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Application.UseCases;

public class CalculateRoutesUseCase
{
    private readonly IRouteCalculator _routeCalculator;

    public CalculateRoutesUseCase(IRouteCalculator routeCalculator)
    {
        _routeCalculator = routeCalculator;
    }

    public IEnumerable<RouteResponseDto> Execute(RouteRequestDto request)
    {
        var routes = _routeCalculator.CalculateTopRoutes(request.FromNodeId, request.ToNodeId, 2);

        return routes.Select(r => new RouteResponseDto
        {
            NodeIds = r.NodeIds,
            TotalCost = r.TotalCost,
            Rank = r.Rank
        });
    }
}