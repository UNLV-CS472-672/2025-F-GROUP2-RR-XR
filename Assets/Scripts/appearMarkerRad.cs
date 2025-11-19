using UnityEngine;
using System.Collections;

/// <summary>
/// ARCHIVED PROTOTYPE: Proximity-based marker appearance system.
/// Originally designed to show waypoint markers based on player proximity using sphere collider triggers.
/// Currently inactive - all functionality has been commented out.
/// Created by: Alex Yamasaki
/// </summary>
/// <remarks>
/// DESIGN DECISION: This prototype was archived because showing waypoint markers based on
/// proximity while navigation is OFF created visual clutter and user confusion.
///
/// Original intended functionality:
/// - Show navigation markers when player enters proximity radius
/// - Hide markers when player exits proximity radius
/// - Only trigger when navigation mode is inactive
///
/// Reason for archival: It was deemed poor UX practice to make waypoints appear
/// spontaneously while users explore the building without active navigation.
/// This could interfere with the navigation system and create a cluttered AR view.
///
/// Current status: Code is kept for reference but all trigger logic is commented out.
/// Consider removing this file if the approach is definitively abandoned.
/// </remarks>
public class appearMarkerRad : MonoBehaviour
{
    /// <summary>
    /// Sphere collider component used for proximity detection.
    /// Configured as a trigger to detect when player enters/exits radius.
    /// </summary>
    SphereCollider collider;

    [Header("Archived Configuration")]
    [SerializeField]
    [Tooltip("Reference to hideMarkers component for controlling marker visibility (currently unused)")]
    private hideMarkers markerManager;

    /// <summary>
    /// Initializes the sphere collider component.
    /// Called once before the first frame update.
    /// </summary>
    void Start()
    {
        // Get sphere collider component for proximity detection
        collider = GetComponent<SphereCollider>();

        // Note: Navigation manager reference was never fully implemented
        // navManager = obj.GetComponent<NavigationManager>();
    }

    /// <summary>
    /// ARCHIVED: Triggered when player enters the proximity radius.
    /// Originally intended to show waypoint markers when navigation is OFF.
    /// All functionality currently commented out.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger zone</param>
    void OnTriggerEnter(Collider collision)
    {
        // ORIGINAL LOGIC (commented out):
        // When navigation isn't active, show the marker when player is within radius

        // if (!markerManager.getnavActive())
        // {
        //     markerManager.showMarker(transform, false);
        // }
    }

    /// <summary>
    /// ARCHIVED: Triggered when player exits the proximity radius.
    /// Originally intended to hide waypoint markers when navigation is OFF.
    /// All functionality currently commented out.
    /// </summary>
    /// <param name="collision">The collider that exited the trigger zone</param>
    void OnTriggerExit(Collider collision)
    {
        // ORIGINAL LOGIC (commented out):
        // Hide markers when player leaves the radius (if navigation is off)

        // if(!markerManager.getnavActive())
        // {
        //     markerManager.hideMarkersVisual();
        // }
    }

    /// <summary>
    /// Called once per frame.
    /// Currently empty - no active update logic.
    /// </summary>
    void Update()
    {
        // No active functionality
    }
}
