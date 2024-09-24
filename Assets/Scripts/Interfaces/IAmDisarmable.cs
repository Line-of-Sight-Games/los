using UnityEngine;

public interface IAmDisarmable
{
    public string Id { get; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public Sprite DisarmImage { get; }
}
