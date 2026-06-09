namespace MiniCiudad.Domain.Entities;

public class CityEdge
{
    public string FromNodeId { get; set; }
    public string ToNodeId { get; set; }
    public int Weight { get; set; }
    public bool IsBlocked { get; set; }
    public string StreetName { get; set; }

    public CityEdge(string fromNodeId, string toNodeId, string streetName, int weight = 1)
    {
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        StreetName = streetName;
        Weight = weight;
        IsBlocked = false;
    }
}