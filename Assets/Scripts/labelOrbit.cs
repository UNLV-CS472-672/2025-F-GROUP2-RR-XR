using UnityEngine;

/// <summary>
/// Controls the orbital rotation behavior of AR labels and signs.
/// Makes labels spin continuously around their vertical axis for visual appeal.
/// Rotation is disabled when the player is close enough, allowing signLabelLook to take over.
/// </summary>
/// <remarks>
/// This component handles:
/// - Continuous rotation around the Y-axis (vertical spinning)
/// - Integration with signLabelLook script to pause rotation when player is nearby
/// - Initial positioning with configurable radius and offset
/// - Smooth rotation using delta time for frame-rate independence
/// </remarks>
public class labelOrbit : MonoBehaviour
{
    [Header("Orbit Configuration")]
    [Tooltip("Center point/transform that the label orbits around (typically the parent marker)")]
    public Transform center;

    [Tooltip("Distance from the center point (currently used for initial offset calculation)")]
    public float radius = 1f;

    [Tooltip("Rotation speed in degrees per second")]
    public float speed = 30f;

    /// <summary>
    /// Current rotation angle in degrees (currently unused but kept for potential future orbital motion).
    /// </summary>
    private float angle = 0f;

    /// <summary>
    /// The axis of rotation - set to Vector3.up for vertical (Y-axis) spinning.
    /// </summary>
    private Vector3 rotationAxis = Vector3.up;

    /// <summary>
    /// Initial position offset from the center point.
    /// Used to position the label at startup.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Reference to the signLabelLook component that handles camera-facing behavior.
    /// When player is close, this script defers to signLabelLook.
    /// </summary>
    private signLabelLook lookScript;

    /// <summary>
    /// Initializes the label's position with an offset from its center point.
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Position label slightly above and away from the center
        offset = new Vector3(0, 0.2f, radius);
        transform.position = center.position + offset;
    }

    /// <summary>
    /// Called once per frame.
    /// Rotates the label around its vertical axis unless the player is close.
    /// </summary>
    void Update()
    {
        // Defer to signLabelLook script when player is nearby
        // This allows the label to face the camera instead of spinning
        if (lookScript != null && lookScript.getPlayerIsClose())
            return;

        // Safety check - stop rotation if center reference is lost
        if (center == null)
            return;

        // Rotate the label around its own vertical axis
        // Uses Space.Self to rotate in local space rather than world space
        transform.Rotate(rotationAxis * speed * Time.deltaTime, Space.Self);

        // NOTE: Alternative orbital motion code commented out below
        // Current implementation rotates in place rather than orbiting around the center

        // TO ORBIT AROUND CERTAIN OBJECT (alternative approach):
        // transform.RotateAround(center.position, Vector3.up, speed * Time.deltaTime);

        // FACE AWAY FROM CENTER (alternative approach):
        // Vector3 direction = transform.position - center.position;
        // if (direction != Vector3.zero)
        //     transform.rotation = Quaternion.LookRotation(direction);

        // LOCK X ROTATION (alternative approach):
        // Vector3 euler = transform.eulerAngles;
        // euler.x = 0f;
        // transform.eulerAngles = euler;
    }
}
