namespace MiniCiudad.Application.DTOs;

public class RouteResponseDto
{
    public List<string> NodeIds { get; set; } = new();
    public int TotalCost { get; set; }
    public int Rank { get; set; }
}