using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    // variable for setting path
    public List<GraphMarker> markers = new List<GraphMarker>(); //all markers
    public List<GraphMarker> path = new List<GraphMarker>(); //calculated path
    public GraphMarker src; //start marker
    public GraphMarker dst; //end marker

    // variables for dijkstra's
    private Dictionary<GraphMarker, float> distances = new Dictionary<GraphMarker, float>(); //stores distance to marker
    private Dictionary<GraphMarker, GraphMarker> prev = new Dictionary<GraphMarker, GraphMarker>(); //stores the back step from a given marker
    private HashSet<GraphMarker> visited = new HashSet<GraphMarker>(); //markers visited
    private List<GraphMarker> unvisited = new List<GraphMarker>(); //minheap

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // use dijkstra's to find a path
    void FindPath()
    {

        // populate
        for (int i = 0; i < markers.Count; i++)
        {
            distances[markers[i]] = float.MaxValue;
            prev[markers[i]] = null;
            unvisited.Add(markers[i]);
        }

        // make src have distance 0 and bring to top of minheap
        distances[src] = 0;
        BubbleUp(unvisited.IndexOf(src));

        while (unvisited.Count > 0)
        {  
            // take marker with min distance
            GraphMarker curr =  unvisited[0];
            unvisited[0] = unvisited[unvisited.Count-1];
            unvisited.RemoveAt(unvisited.Count-1);
            BubbleDown(0);

            // if already visited go to next
            if (visited.Contains(curr))
                continue;

            visited.Add(curr);

            // if destination, done
            if (curr == dst)
                break;

            // loop through all markers connected to curr
            for (int i = 0; i < curr.links.Count; i++)
            {
                if (visited.Contains(curr.links[i]))
                    continue;
                
                // get dist from curr to neighbor
                float dist = Vector3.Distance(curr.transform.position, curr.links[i].transform.position);
                float totalDist = distances[curr] + dist;

                if (totalDist < distances[curr.links[i]])
                {
                    // update dist
                    distances[curr.links[i]] = totalDist;
                    prev[curr.links[i]] = curr;

                    // update heap
                    int n = unvisited.IndexOf(curr.links[i]);
                    if (n >= 0)
                        BubbleUp(n);
                }
            }

        }

        //form path backward from dst
        GraphMarker step = dst;
        while (step != null)
        {
            {
                path.Insert(0, step);
                step = prev[step];
            }
        }
    }

    // move marker up the heap (inserting)
    void BubbleUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1)/2;
            if (distances[unvisited[i]] >= distances[unvisited[parent]])
                break;
            
            (unvisited[i], unvisited[parent]) = (unvisited[parent], unvisited[i]);
            i = parent;
        }
    }

    // move marker down the heap (deleting)
    void BubbleDown(int i)
    {
        int last = unvisited.Count - 1;
        int left = 2 * i + 1;
        while (left < unvisited.Count)
        {
            int right = 2 * i + 2;

            if (right < unvisited.Count && distances[unvisited[right]] < distances[unvisited[left]])
                left = right;
            if (distances[unvisited[left]] < distances[unvisited[i]])
            {
                (unvisited[left], unvisited[i]) = (unvisited[i], unvisited[left]);
                i = left;
                left = 2 * i + 1;
            }
            else
                break;
        }
    }
}
