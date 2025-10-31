using UnityEngine;

public class hideMarkers : MonoBehaviour
{
    //hide markers when program starts based on the parent object
    [SerializeField]

    private GameObject parentObj;
    //variable tag
    [SerializeField]
    private string markerTag = "markerMesh";

    //variables
    private bool navActive = false;

    void Start()
    {

        hideMarkersVisual();
    }
    public void toggleNavActive()
    {
        navActive = !navActive;
       
         
    }
    public bool getnavActive()
    {
        return navActive;
    }
    public void hideMarkersVisual()
    {
        //iterate through parent objects and hide them 
        //via function usage of .SetActive(false)
        foreach (Transform child in parentObj.transform)
        {
            //Find sub-children containing the tag of "markerMesh"
            if (child.CompareTag(markerTag))
            {
                foreach (Transform subchild in child)
                {
                    subchild.gameObject.SetActive(false);
                }
            }

        }
    }
    public void showMarker(Transform target, bool navOption)
    {
        //call in the function to hide all the markers

        hideMarkersVisual();
        
        if (navOption)
            toggleNavActive();
        //show the selected one and make it show.
        foreach (Transform child in target)
        {
            child.gameObject.SetActive(true);
        }
    }

 
}
