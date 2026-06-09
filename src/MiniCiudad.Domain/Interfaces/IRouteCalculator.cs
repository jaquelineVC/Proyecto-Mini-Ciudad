using MiniCiudad.Domain.Entities;

namespace MiniCiudad.Domain.Interfaces;

public interface IRouteCalculator
{
    IEnumerable<CityRoute> CalculateTopRoutes(string fromNodeId, string toNodeId, int topCount = 2);
}