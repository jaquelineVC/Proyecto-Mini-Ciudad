namespace MiniCiudad.Application.DTOs;

public class RouteRequestDto
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
}