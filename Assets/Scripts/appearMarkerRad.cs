using UnityEngine;
using System.Collections;
//Script created by Alex Yamasaki
public class appearMarkerRad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SphereCollider collider;
    [SerializeField]
    private hideMarkers markerManager;

   
    void Start()
    {
        //componenet of spherecollider
        collider = GetComponent<SphereCollider>();
        //navManager = obj.GetComponent<NavigationManager>();

    }
    //OnTriggerEnter of the object when it collides with player (camera) and target
    //            something would trigger, in this case; it would appear the waypoint
    //            WHEN navigation is off. 
    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("TEST");
        //if navigation isn't active, we can make an assumption that it can popup
        //when user is closed to the object-radius
        if (!markerManager.getnavActive())
        {
            markerManager.showMarker(transform, false);
        }


    }
    //onCollisionExit of the object when the player gets outside of the waypoint boundary
    //                then it would hide the marker IF navigation mode is off.
    void OnTriggerExit(Collider collision)
    {
        //Debug.Log("HIDE");
        //Debug.Log(markerManager.getnavActive)
        if(!markerManager.getnavActive())
        {
            //Debug.Log("TRIGGERED");
            markerManager.hideMarkersVisual();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
