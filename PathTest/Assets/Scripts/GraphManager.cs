using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public List<GraphMarker> markers = new List<GraphMarker>();
    public List<GraphMarker> path = new List<GraphMarker>();
    public GraphMarker src;
    public GraphMarker dst;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FindPath()
    {
        Dictionary<GraphMarker, float> distances = new Dictionary<GraphMarker, float>();
        Dictionary<GraphMarker, GraphMarker> prev = new Dictionary<GraphMarker, GraphMarker>();
        HashSet<GraphMarker> visited = new HashSet<GraphMarker>();

        for (int i = 0; i < markers.Count; i++)
        {
            distances[markers[i]] = float.MaxValue;
            prev[markers[i]] = null;
        }

        distances[src] = 0;
        List<GraphMarker> unvisited = new List<GraphMarker>(markers);

        while (unvisited.Count > 0)
        {  
            //sort to get closest unvisited marker
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            GraphMarker curr = unvisited[0];
            unvisited.RemoveAt(0);

            //break if it's the destination
            if (curr == dst)
                break;

            visited.Add(curr);
            for (int i = 0; i < curr.links.Count; i++)
            {
                if (visited.Contains(curr.links[i]))
                    continue;
                
                float dist = Vector3.Distance(curr.transform.position, curr.links[i].transform.position);
                float totalDist = distances[curr] + dist;

                if (totalDist < distances[curr.links[i]])
                {
                    distances[curr.links[i]] = totalDist;
                    prev[curr.links[i]] = curr;
                }
            }

        }

        //form path backward from dst
        GraphMarker step = dst;
        if (prev[step] != null || step == src)
        {
            while (step != null)
            {
                path.Insert(0, step);
                step = prev[step];
            }
        }
    }
}
