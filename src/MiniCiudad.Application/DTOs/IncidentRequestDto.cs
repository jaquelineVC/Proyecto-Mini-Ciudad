namespace MiniCiudad.Application.DTOs;

public class IncidentRequestDto
{
    public string Type { get; set; } = string.Empty;        // "RoadClosed" | "TrafficJam"
    public string Scope { get; set; } = string.Empty;       // "SingleSegment" | "FullStreet"
    public string Description { get; set; } = string.Empty;

    // Para SingleSegment
    public string? FromNodeId { get; set; }
    public string? ToNodeId { get; set; }

    // Para FullStreet
    public string? StreetName { get; set; }
}