namespace MiniCiudad.Domain.Entities;

public enum IncidentType
{
    RoadClosed,
    TrafficJam
}

public enum IncidentScope
{
    SingleSegment,
    FullStreet
}

public class Incident
{
    public Guid Id { get; set; }
    public string? FromNodeId { get; set; }
    public string? ToNodeId { get; set; }
    public string? StreetName { get; set; }
    public IncidentType Type { get; set; }
    public IncidentScope Scope { get; set; }
    public string Description { get; set; }
    public DateTime ReportedAt { get; set; }

    public Incident(
        IncidentType type,
        IncidentScope scope,
        string description,
        string? fromNodeId = null,
        string? toNodeId = null,
        string? streetName = null)
    {
        Id = Guid.NewGuid();
        Type = type;
        Scope = scope;
        Description = description;
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        StreetName = streetName;
        ReportedAt = DateTime.UtcNow;
    }
}