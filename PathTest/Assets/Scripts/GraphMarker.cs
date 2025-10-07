using System.Collections.Generic;
using UnityEngine;

public class GraphMarker : MonoBehaviour
{
    public string id;
    public List<GraphMarker> links = new List<GraphMarker>();
}
