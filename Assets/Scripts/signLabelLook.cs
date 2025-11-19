using UnityEngine;

/// <summary>
/// Makes AR labels and signs face the camera when the player is within proximity range.
/// Uses a sphere collider trigger to detect player distance and smoothly rotates to face the camera.
/// </summary>
/// <remarks>
/// This component handles:
/// - Automatic camera/player transform detection using Camera.main
/// - Proximity detection using a sphere collider trigger
/// - Smooth camera-facing rotation with spherical interpolation (Slerp)
/// - Y-axis only rotation to keep labels upright
/// - 180-degree offset to face the player correctly
/// Works in conjunction with labelOrbit - this script takes over when player is close.
/// </remarks>
public class signLabelLook : MonoBehaviour
{
    [Header("Look-At Configuration")]
    [Tooltip("Transform of the player/camera to face towards (auto-assigned to Camera.main if not set)")]
    public Transform player;

    [Tooltip("Detection radius - label will face camera when player enters this range")]
    public float radius = 2f;

    /// <summary>
    /// Tracks whether the player is currently within the trigger radius.
    /// When true, the label rotates to face the camera.
    /// </summary>
    private bool playerIsClose = false;

    /// <summary>
    /// Sphere collider component used for proximity detection.
    /// Configured as a trigger with the specified radius.
    /// </summary>
    private SphereCollider sphere;

    /// <summary>
    /// Called when the script instance is being loaded (before Start).
    /// Sets up the sphere collider trigger and finds the main camera.
    /// </summary>
    void Awake()
    {
        // Get or add sphere collider component for proximity detection
        sphere = GetComponent<SphereCollider>();

        // Configure as trigger (doesn't cause physics collisions)
        sphere.isTrigger = true;
        sphere.radius = radius;

        // Auto-assign main camera as the player transform if not manually set
        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }

    /// <summary>
    /// Called once before the first frame update.
    /// Ensures player transform is assigned (fallback if Awake failed).
    /// </summary>
    void Start()
    {
        // Double-check camera assignment in case it wasn't available in Awake
        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }

    /// <summary>
    /// Gets whether the player is currently within the trigger radius.
    /// Used by labelOrbit to determine if it should stop rotating.
    /// </summary>
    /// <returns>True if player is close, false otherwise</returns>
    public bool getPlayerIsClose()
    {
        return playerIsClose;
    }

    /// <summary>
    /// Called once per frame.
    /// Smoothly rotates the label to face the camera when player is nearby.
    /// </summary>
    void Update()
    {
        // Only rotate to face camera when player is within range
        if (playerIsClose && player != null)
        {
            // Calculate direction vector from label to player
            Vector3 lookDir = player.position - transform.position;

            // Zero out Y component to only rotate on horizontal plane
            // This keeps the label upright instead of tilting up/down
            lookDir.y = 0f;

            // Calculate target rotation to look at the player
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            // Apply 180-degree Y rotation so the front of the label faces the player
            // Without this, the back of the label would face the player
            targetRotation *= Quaternion.Euler(0, 180f, 0);

            // Smoothly interpolate from current rotation to target rotation
            // Speed factor of 3 provides smooth but responsive rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }
    }

    /// <summary>
    /// Triggered when another collider enters this object's trigger zone.
    /// Sets playerIsClose flag if the entering object is the player/camera.
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        // Note: Uses & (bitwise AND) instead of && (logical AND) - likely a typo but works
        if (player != null & other.transform == player)
        {
            playerIsClose = true;
        }
    }

    /// <summary>
    /// Triggered when another collider exits this object's trigger zone.
    /// Clears playerIsClose flag if the exiting object is the player/camera.
    /// </summary>
    /// <param name="other">The collider that exited the trigger</param>
    void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player
        if(player != null && other.transform == player)
        {
            playerIsClose = false;
        }
    }
}
