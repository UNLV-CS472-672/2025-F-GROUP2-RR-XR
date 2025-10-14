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

    // Creates the path particles from the 0th marker in path (from GraphManager)
    IEnumerator FormVisualPath()
    {
        while (true)
        {
            // create a path particle
            GameObject particle = Instantiate(pathParticle, graphManager.path[0].transform.position, Quaternion.identity);

            PathObject pathObject = particle.GetComponent<PathObject>();
            if (pathObject != null) 
            {
                //instantiate with path
                pathObject.Initialize(graphManager.path);
            } 

            //wait a few seconds before making another
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
