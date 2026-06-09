using MiniCiudad.Domain.Entities;

namespace MiniCiudad.Domain.Interfaces;

public interface IGraphRepository
{
    IEnumerable<CityNode> GetAllNodes();
    IEnumerable<CityEdge> GetAllEdges();
    IEnumerable<CityEdge> GetEdgesFrom(string nodeId);
    IEnumerable<string> GetAllStreetNames();
    void ApplyIncident(Incident incident);
    void RemoveIncident(Guid incidentId);
    IEnumerable<Incident> GetActiveIncidents();
}