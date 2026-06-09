namespace MiniCiudad.Domain.Entities;

public class CityNode
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string Id => $"{Row}-{Col}";

    public CityNode(int row, int col)
    {
        Row = row;
        Col = col;
    }
}