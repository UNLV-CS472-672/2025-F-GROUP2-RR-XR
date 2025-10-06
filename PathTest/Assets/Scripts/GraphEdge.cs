using UnityEngine;

public class GraphEdge
{
    public GraphMarker from;
    public GraphMarker to;
    public float dist;

    public GraphEdge(GraphMarker from, GraphMarker to, float dist)
    {
        this.from = from;
        this.to = to;
        this.dist = dist;
    }
}
