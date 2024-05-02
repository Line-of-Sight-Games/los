using System.Collections.Generic;

public interface IAmDetectable
{
    public string Id { get; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public int ActiveC { get; }
    public int ActiveF { get; }
}
