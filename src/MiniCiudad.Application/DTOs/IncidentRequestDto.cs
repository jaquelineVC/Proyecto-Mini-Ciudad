namespace MiniCiudad.Application.DTOs;

public class IncidentRequestDto
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "RoadClosed" | "TrafficJam"
    public string Description { get; set; } = string.Empty;
}