using System.Collections.Generic;
using UnityEngine;

public class PathObject : MonoBehaviour
{
    public float speed = 2.0f;
    private List<GraphMarker> markers;
    private int index = 0;
    private bool initialized = false;

    public void Initialize(List<GraphMarker> markers)
    {
        this.markers = markers;
        index = 0;
        initialized = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized || markers == null || index >= markers.Count - 1)
            return;

        Vector3 target = markers[index + 1].transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            index++;
            if (index >= markers.Count - 1)
                Destroy(gameObject);
        }
    }
}
