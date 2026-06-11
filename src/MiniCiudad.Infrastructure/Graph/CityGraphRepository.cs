using MiniCiudad.Domain.Entities;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Infrastructure.Graph;

public class CityGraphRepository : IGraphRepository
{
    private readonly CityNode[,] _nodes;
    private readonly LinkedList<CityEdge> _edges;
    private readonly Dictionary<Guid, Incident> _incidents;
    private readonly Dictionary<string, Dictionary<string, int>> _baseWeights;

    private const int ROWS = 4;
    private const int COLS = 7;
    private const int TRAFFIC_MULTIPLIER = 5;

    // Nombres de calles
    private static readonly string[] AvenueNames =
    [
        "Av. Juárez",        // Col 0 ↓
        "Av. Hidalgo",       // Col 1 ↑
        "Av. Morelos",       // Col 2 ↓
        "Av. Reforma",       // Col 3 ↑
        "Av. Madero",        // Col 4 ↓
        "Av. Zaragoza",      // Col 5 ↑
        "Av. Independencia"  // Col 6 ↓
    ];

    private static readonly string[] StreetNames =
    [
        "Calle Libertad",   // Fila 0 →
        "Calle Aldama",     // Fila 1 ←
        "Calle Allende",    // Fila 2 →
        "Calle Guerrero"    // Fila 3 ←
    ];

    public CityGraphRepository()
    {
        _nodes = new CityNode[ROWS, COLS];
        _edges = new LinkedList<CityEdge>();
        _incidents = new Dictionary<Guid, Incident>();
        _baseWeights = new Dictionary<string, Dictionary<string, int>>();

        InitializeNodes();
        InitializeEdges();
    }

    private void InitializeNodes()
    {
        for (int row = 0; row < ROWS; row++)
            for (int col = 0; col < COLS; col++)
                _nodes[row, col] = new CityNode(row, col);
    }

    private void InitializeEdges()
    {
        // Calles VERTICALES (Avenidas)
        // Columnas pares bajan ↓, impares suben ↑
        for (int col = 0; col < COLS; col++)
        {
            bool goesDown = col % 2 == 0;
            string avenueName = AvenueNames[col];

            for (int row = 0; row < ROWS - 1; row++)
            {
                string from = goesDown
                    ? _nodes[row, col].Id
                    : _nodes[ROWS - 1 - row, col].Id;

                string to = goesDown
                    ? _nodes[row + 1, col].Id
                    : _nodes[ROWS - 2 - row, col].Id;

                AddEdge(from, to, avenueName);
            }
        }

        // Calles HORIZONTALES
        // Filas pares van a la derecha →, impares a la izquierda ←
        for (int row = 0; row < ROWS; row++)
        {
            bool goesRight = row % 2 == 0;
            string streetName = StreetNames[row];

            for (int col = 0; col < COLS - 1; col++)
            {
                string from = goesRight
                    ? _nodes[row, col].Id
                    : _nodes[row, COLS - 1 - col].Id;

                string to = goesRight
                    ? _nodes[row, col + 1].Id
                    : _nodes[row, COLS - 2 - col].Id;

                AddEdge(from, to, streetName);
            }
        }
    }

    private void AddEdge(string from, string to, string streetName, int weight = 1)
    {
        _edges.AddLast(new CityEdge(from, to, streetName, weight));

        if (!_baseWeights.ContainsKey(from))
            _baseWeights[from] = new Dictionary<string, int>();

        _baseWeights[from][to] = weight;
    }

    public IEnumerable<CityNode> GetAllNodes()
    {
        var list = new List<CityNode>();
        for (int row = 0; row < ROWS; row++)
            for (int col = 0; col < COLS; col++)
                list.Add(_nodes[row, col]);
        return list;
    }

    public IEnumerable<CityEdge> GetAllEdges() => _edges;

    public IEnumerable<CityEdge> GetEdgesFrom(string nodeId)
    {
        return _edges.Where(e => e.FromNodeId == nodeId && !e.IsBlocked);
    }

    public IEnumerable<string> GetAllStreetNames()
    {
        return AvenueNames.Concat(StreetNames).Order();
    }

    public void ApplyIncident(Incident incident)
    {
        _incidents[incident.Id] = incident;

        if (incident.Scope == IncidentScope.FullStreet)
        {
            ApplyToFullStreet(incident);
        }
        else
        {
            ApplyToSegment(incident.FromNodeId!, incident.ToNodeId!, incident.Type);
        }
    }

    private void ApplyToFullStreet(Incident incident)
    {
        var affectedEdges = _edges.Where(e => e.StreetName == incident.StreetName);

        foreach (var edge in affectedEdges)
        {
            if (incident.Type == IncidentType.RoadClosed)
            {
                edge.IsBlocked = true;
            }
            else if (incident.Type == IncidentType.TrafficJam)
            {
                edge.Weight = _baseWeights[edge.FromNodeId][edge.ToNodeId] * TRAFFIC_MULTIPLIER;
            }
        }
    }

    private void ApplyToSegment(string fromNodeId, string toNodeId, IncidentType type)
    {
        var edge = _edges.FirstOrDefault(e =>
            e.FromNodeId == fromNodeId && e.ToNodeId == toNodeId);

        if (edge == null) return;

        if (type == IncidentType.RoadClosed)
        {
            edge.IsBlocked = true;
        }
        else if (type == IncidentType.TrafficJam)
        {
            edge.Weight = _baseWeights[fromNodeId][toNodeId] * TRAFFIC_MULTIPLIER;
        }
    }

    public void RemoveIncident(Guid incidentId)
    {
        if (!_incidents.TryGetValue(incidentId, out var incident)) return;

        if (incident.Scope == IncidentScope.FullStreet)
        {
            var affectedEdges = _edges.Where(e => e.StreetName == incident.StreetName);
            foreach (var edge in affectedEdges)
            {
                edge.IsBlocked = false;
                edge.Weight = _baseWeights[edge.FromNodeId][edge.ToNodeId];
            }
        }
        else
        {
            var edge = _edges.FirstOrDefault(e =>
                e.FromNodeId == incident.FromNodeId &&
                e.ToNodeId == incident.ToNodeId);

            if (edge != null)
            {
                edge.IsBlocked = false;
                edge.Weight = _baseWeights[incident.FromNodeId!][incident.ToNodeId!];
            }
        }

        _incidents.Remove(incidentId);
    }

    public IEnumerable<Incident> GetActiveIncidents() => _incidents.Values;
}