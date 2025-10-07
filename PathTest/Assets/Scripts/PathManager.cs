using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public GameObject pathParticle;
    public float spawnSpeed = 1.0f;
    public GraphManager graphManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FormVisualPath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FormVisualPath()
    {
        while (true)
        {
            GameObject particle = Instantiate(pathParticle, graphManager.path[0].transform.position, Quaternion.identity);

            PathObject pathObject = particle.GetComponent<PathObject>();
            if (pathObject != null) 
            {
                pathObject.Initialize(graphManager.path);
            } 

            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
