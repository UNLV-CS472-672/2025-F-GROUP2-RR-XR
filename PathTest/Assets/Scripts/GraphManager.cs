using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public List<GraphMarker> markers = new List<GraphMarker>();
    public List<GraphEdge> edges = new List<GraphEdge>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGraph()
    {
        for (int i = 0; i < markers.Count; i++) 
        {
            for (int j = i+1; j < markers.Count; j++) 
            {
                float dist = Vector3.Distance(markers[i].transform.position, markers[j].transform.position);
                edges.Add(new GraphEdge(markers[i], markers[j], dist));
                edges.Add(new GraphEdge(markers[j], markers[i], dist));
            }
        }
    }
}
