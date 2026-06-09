namespace MiniCiudad.Domain.Entities;

public enum IncidentType
{
    RoadClosed,
    TrafficJam
}

public class Incident
{
    public Guid Id { get; set; }
    public string FromNodeId { get; set; }
    public string ToNodeId { get; set; }
    public IncidentType Type { get; set; }
    public string Description { get; set; }
    public DateTime ReportedAt { get; set; }

    public Incident(string fromNodeId, string toNodeId, IncidentType type, string description)
    {
        Id = Guid.NewGuid();
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        Type = type;
        Description = description;
        ReportedAt = DateTime.UtcNow;
    }
}