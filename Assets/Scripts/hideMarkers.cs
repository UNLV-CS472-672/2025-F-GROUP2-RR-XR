using UnityEngine;

public class hideMarkers : MonoBehaviour
{
    //hide markers when program starts based on the parent object
    [SerializeField]

    private GameObject parentObj;
    //variable tag
    [SerializeField]
    private string markerTag = "markerMesh";
    void Start()
    {

        hideMarkersVisual();
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
    public void showMarker(Transform target)
    {
        hideMarkersVisual();
      
        //show the selected one and make it show.
        foreach(Transform child in target)
        {
            child.gameObject.SetActive(true);
        }
    }
 
}
