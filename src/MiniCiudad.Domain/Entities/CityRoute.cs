namespace MiniCiudad.Domain.Entities;

public class CityRoute
{
    public List<string> NodeIds { get; set; }
    public int TotalCost { get; set; }
    public int Rank { get; set; }

    public CityRoute(List<string> nodeIds, int totalCost, int rank)
    {
        NodeIds = nodeIds;
        TotalCost = totalCost;
        Rank = rank;
    }
}