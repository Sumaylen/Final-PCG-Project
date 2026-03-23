using UnityEngine;

public class BSPNode 
{
    public RectInt partition;
    public BSPNode firstChild;
    public BSPNode secondChild;
    public RectInt room;

    public BSPNode(RectInt partition)
    {
        this.partition = partition;
    }

    public bool IsLeaf()
    {
        return firstChild == null && secondChild == null;
    }
}
