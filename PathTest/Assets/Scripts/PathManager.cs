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
        StartCoroutine(FormPath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FormPath()
    {
        while (true)
        {
            GameObject particle = Instantiate(pathParticle, graphManager.markers[0].transform.position, Quaternion.identity);

            PathObject pathObject = particle.GetComponent<PathObject>();
            if (pathObject != null) 
            {
                pathObject.Initialize(graphManager.markers);
            } 

            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
