using System.Collections.Generic;

public interface IExplosive
{
    public string Id { get; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}