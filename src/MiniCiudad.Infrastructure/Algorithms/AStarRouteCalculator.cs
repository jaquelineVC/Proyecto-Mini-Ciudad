using MiniCiudad.Domain.Entities;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Infrastructure.Algorithms;

public class AStarRouteCalculator : IRouteCalculator
{
    private readonly IGraphRepository _graphRepository;

    public AStarRouteCalculator(IGraphRepository graphRepository)
    {
        _graphRepository = graphRepository;
    }

    public IEnumerable<CityRoute> CalculateTopRoutes(string fromNodeId, string toNodeId, int topCount = 2)
    {
        var routes = new List<CityRoute>();
        var candidates = new List<(int cost, List<string> path)>();

        var firstResult = AStar(fromNodeId, toNodeId);
        if (firstResult == null) return routes;

        var firstPath = firstResult.Value.path;
        var firstCost = firstResult.Value.cost;

        routes.Add(new CityRoute(firstPath, firstCost, 1));

        if (topCount < 2) return routes;

        for (int i = 0; i < firstPath.Count - 1; i++)
        {
            var edgeFrom = firstPath[i];
            var edgeTo = firstPath[i + 1];

            var edge = _graphRepository.GetAllEdges()
                .FirstOrDefault(e => e.FromNodeId == edgeFrom && e.ToNodeId == edgeTo);

            if (edge == null) continue;

            edge.IsBlocked = true;

            var altResult = AStar(fromNodeId, toNodeId);

            edge.IsBlocked = false;

            if (altResult != null && !PathEquals(altResult.Value.path, firstPath))
                candidates.Add((altResult.Value.cost, altResult.Value.path));
        }

        if (candidates.Count > 0)
        {
            var best = candidates.OrderBy(c => c.cost).First();
            routes.Add(new CityRoute(best.path, best.cost, 2));
        }

        return routes;
    }

    private (List<string> path, int cost)? AStar(string startId, string goalId)
    {
        var allNodes = _graphRepository.GetAllNodes().ToDictionary(n => n.Id);

        if (!allNodes.ContainsKey(startId) || !allNodes.ContainsKey(goalId))
            return null;

        var goal = allNodes[goalId];

        var gScore = new Dictionary<string, int> { [startId] = 0 };
        var fScore = new Dictionary<string, int> { [startId] = Heuristic(allNodes[startId], goal) };
        var cameFrom = new Dictionary<string, string>();

        var openSet = new SortedSet<(int f, string id)>(Comparer<(int f, string id)>.Create((a, b) =>
            a.f != b.f ? a.f.CompareTo(b.f) : string.Compare(a.id, b.id, StringComparison.Ordinal)));

        openSet.Add((fScore[startId], startId));

        while (openSet.Count > 0)
        {
            var current = openSet.Min;
            openSet.Remove(current);
            var currentId = current.id;

            if (currentId == goalId)
                return (ReconstructPath(cameFrom, currentId), gScore[currentId]);

            foreach (var edge in _graphRepository.GetEdgesFrom(currentId))
            {
                var neighborId = edge.ToNodeId;
                var tentativeG = gScore[currentId] + edge.Weight;

                if (!gScore.ContainsKey(neighborId) || tentativeG < gScore[neighborId])
                {
                    cameFrom[neighborId] = currentId;
                    gScore[neighborId] = tentativeG;

                    var h = allNodes.ContainsKey(neighborId)
                        ? Heuristic(allNodes[neighborId], goal)
                        : 0;

                    var f = tentativeG + h;
                    fScore[neighborId] = f;

                    openSet.RemoveWhere(x => x.id == neighborId);
                    openSet.Add((f, neighborId));
                }
            }
        }

        return null;
    }

    private static int Heuristic(CityNode a, CityNode b)
    {
        return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
    }

    private static List<string> ReconstructPath(Dictionary<string, string> cameFrom, string current)
    {
        var path = new List<string> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private static bool PathEquals(List<string> a, List<string> b)
    {
        if (a.Count != b.Count) return false;
        return !a.Where((t, i) => t != b[i]).Any();
    }
}