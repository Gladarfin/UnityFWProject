using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 Position { get; }
    public int VertexIndex { get; set; } = -1;

    public Node(Vector3 pos)
    {
        Position = pos;
    }
}
