using UnityEngine;

/// <summary>
/// Manages the visibility of navigation markers and waypoints in the AR environment.
/// Controls which markers are visible based on navigation state and user proximity.
/// </summary>
/// <remarks>
/// This component handles:
/// - Hiding all navigation markers on startup
/// - Toggling navigation mode state
/// - Showing specific markers when selected for navigation
/// - Managing marker visibility based on parent-child hierarchy
/// </remarks>
public class hideMarkers : MonoBehaviour
{
    [Header("Marker Configuration")]
    [SerializeField]
    [Tooltip("Parent GameObject containing all navigation markers as children")]
    private GameObject parentObj;

    [SerializeField]
    [Tooltip("Tag used to identify marker mesh objects (default: 'markerMesh')")]
    private string markerTag = "markerMesh";

    /// <summary>
    /// Tracks whether navigation mode is currently active.
    /// When true, waypoint markers are shown for active navigation.
    /// </summary>
    private bool navActive = false;

    /// <summary>
    /// Initializes the marker manager by hiding all markers on startup.
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Hide all markers initially to prevent visual clutter
        hideMarkersVisual();
    }

    /// <summary>
    /// Toggles the navigation active state between on and off.
    /// Used to track whether the user is actively navigating to a destination.
    /// </summary>
    public void toggleNavActive()
    {
        navActive = !navActive;
    }

    /// <summary>
    /// Gets the current navigation active state.
    /// </summary>
    /// <returns>True if navigation mode is active, false otherwise</returns>
    public bool getnavActive()
    {
        return navActive;
    }

    /// <summary>
    /// Hides all navigation markers by deactivating all child objects under the parent.
    /// Iterates through the entire hierarchy and sets all markers to inactive.
    /// </summary>
    public void hideMarkersVisual()
    {
        // Iterate through all direct children of the parent object
        foreach (Transform child in parentObj.transform)
        {
            // Iterate through sub-children (the actual marker meshes)
            // and deactivate them to hide from view
            foreach (Transform subchild in child)
            {
                subchild.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows a specific navigation marker while hiding all others.
    /// Optionally toggles navigation mode when showing the marker.
    /// </summary>
    /// <param name="target">Transform of the marker to show</param>
    /// <param name="navOption">If true, toggles navigation active state</param>
    public void showMarker(Transform target, bool navOption)
    {
        // First hide all existing markers to ensure only one is visible
        hideMarkersVisual();

        // Toggle navigation mode if requested
        if (navOption)
            toggleNavActive();

        // Show all children of the target marker
        // This includes the mesh and any associated visual elements
        foreach (Transform child in target)
        {
            child.gameObject.SetActive(true);
        }
    }
}
