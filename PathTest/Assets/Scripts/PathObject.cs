using System.Collections.Generic;
using UnityEngine;

public class PathObject : MonoBehaviour
{
    public float speed = 5.0f;
    private List<GraphMarker> path;
    private int index = 0;
    private bool initialized = false;

    public void Initialize(List<GraphMarker> path)
    {
        this.path = path;
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
        if (!initialized || path == null || index >= path.Count - 1)
            return;

        // move towards next marker
        Vector3 target = path[index + 1].transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // when reaching the next marker, switch target to the next marker
        // if it's the final marker in the path, destroy self
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            index++;
            if (index >= path.Count - 1)
                Destroy(gameObject);
        }
    }
}
