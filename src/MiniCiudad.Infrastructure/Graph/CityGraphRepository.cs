using MiniCiudad.Domain.Entities;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Infrastructure.Graph;

public class CityGraphRepository : IGraphRepository
{
    // Nodos: array 2D de 7 columnas x 4 filas = 28 nodos
    private readonly CityNode[,] _nodes;

    // Aristas: lista encadenada representada como LinkedList
    private readonly LinkedList<CityEdge> _edges;

    // Incidencias activas
    private readonly Dictionary<Guid, Incident> _incidents;

    // Pesos base de tráfico por arista (fromNodeId -> toNodeId -> peso)
    private readonly Dictionary<string, Dictionary<string, int>> _baseWeights;

    private const int ROWS = 4;
    private const int COLS = 7;
    private const int TRAFFIC_MULTIPLIER = 5;

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
        // Calles VERTICALES (sentido por columna)
        // Col 0: ↓ baja (fila 0 -> fila 1 -> fila 2 -> fila 3)
        // Col 1: ↑ sube (fila 3 -> fila 2 -> fila 1 -> fila 0)
        // Col 2: ↓ baja
        // Col 3: ↑ sube
        // Col 4: ↓ baja
        // Col 5: ↑ sube
        // Col 6: ↓ baja

        for (int col = 0; col < COLS; col++)
        {
            bool goesDown = col % 2 == 0; // cols pares bajan, impares suben

            for (int row = 0; row < ROWS - 1; row++)
            {
                string from = goesDown
                    ? _nodes[row, col].Id
                    : _nodes[ROWS - 1 - row, col].Id;

                string to = goesDown
                    ? _nodes[row + 1, col].Id
                    : _nodes[ROWS - 2 - row, col].Id;

                AddEdge(from, to);
            }
        }

        // Calles HORIZONTALES (sentido por fila)
        // Fila 0: → derecha
        // Fila 1: ← izquierda
        // Fila 2: → derecha
        // Fila 3: ← izquierda

        for (int row = 0; row < ROWS; row++)
        {
            bool goesRight = row % 2 == 0; // filas pares van a la derecha, impares a la izquierda

            for (int col = 0; col < COLS - 1; col++)
            {
                string from = goesRight
                    ? _nodes[row, col].Id
                    : _nodes[row, COLS - 1 - col].Id;

                string to = goesRight
                    ? _nodes[row, col + 1].Id
                    : _nodes[row, COLS - 2 - col].Id;

                AddEdge(from, to);
            }
        }
    }

    private void AddEdge(string from, string to, int weight = 1)
    {
        _edges.AddLast(new CityEdge(from, to, weight));

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

    public void ApplyIncident(Incident incident)
    {
        _incidents[incident.Id] = incident;

        var edge = _edges.FirstOrDefault(e =>
            e.FromNodeId == incident.FromNodeId &&
            e.ToNodeId == incident.ToNodeId);

        if (edge == null) return;

        if (incident.Type == IncidentType.RoadClosed)
        {
            edge.IsBlocked = true;
        }
        else if (incident.Type == IncidentType.TrafficJam)
        {
            edge.Weight = _baseWeights[incident.FromNodeId][incident.ToNodeId] * TRAFFIC_MULTIPLIER;
        }
    }

    public void RemoveIncident(Guid incidentId)
    {
        if (!_incidents.TryGetValue(incidentId, out var incident)) return;

        var edge = _edges.FirstOrDefault(e =>
            e.FromNodeId == incident.FromNodeId &&
            e.ToNodeId == incident.ToNodeId);

        if (edge != null)
        {
            edge.IsBlocked = false;
            edge.Weight = _baseWeights[incident.FromNodeId][incident.ToNodeId];
        }

        _incidents.Remove(incidentId);
    }

    public IEnumerable<Incident> GetActiveIncidents() => _incidents.Values;
}