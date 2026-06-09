namespace MiniCiudad.Application.DTOs;

public class IncidentResponseDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FromNodeId { get; set; }
    public string? ToNodeId { get; set; }
    public string? StreetName { get; set; }
    public DateTime ReportedAt { get; set; }
}