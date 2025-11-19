using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dynamically creates and manages 3D text labels for AR markers and waypoints.
/// Labels appear when the player enters a trigger zone and disappear on exit or during navigation.
/// Created by: Alex Yamasaki
/// </summary>
/// <remarks>
/// This component handles:
/// - Creating 3D TextMeshPro labels on trigger enter events
/// - Configuring label appearance (font, size, color, positioning)
/// - Adding orbital rotation and camera-facing behavior to labels
/// - Destroying labels when player exits trigger zone or navigation mode activates
/// - Integration with the hideMarkers navigation system
/// </remarks>
public class createLabel : MonoBehaviour
{
    [Header("Label Configuration")]
    [SerializeField]
    [Tooltip("Parent GameObject that the label will be attached to (typically a marker or waypoint)")]
    public GameObject parentObject;

    [SerializeField]
    [Tooltip("Prefab template for the label's 3D sign shape/background")]
    public GameObject labelPrefab;

    [SerializeField]
    [Tooltip("Vertical offset of the label above its parent object (in local space units)")]
    public float labelHeightOffset = 0.3f;

    /// <summary>
    /// Reference to the currently instantiated label GameObject.
    /// Used for tracking and cleanup.
    /// </summary>
    private GameObject currentLabel;

    /// <summary>
    /// Reference to the parent hideMarkers component that manages navigation state.
    /// Used to determine whether navigation mode is active.
    /// </summary>
    private hideMarkers parentManager;

    /// <summary>
    /// Initializes the label creator by finding the parent hideMarkers manager.
    /// Called once before the first frame update.
    /// </summary>
    void Start()
    {
        // Get reference to navigation manager to check navigation state
        parentManager = GetComponentInParent<hideMarkers>();
    }

    /// <summary>
    /// Creates a new 3D label with text displaying the target object's name.
    /// Configures the label with TextMeshPro, orbital rotation, and camera-facing behavior.
    /// </summary>
    /// <param name="target">Transform of the object to create a label for</param>
    private void labelCreate(Transform target)
    {
        // Instantiate the label prefab as a child of the target
        // This creates the visual background/sign shape
        GameObject label = Instantiate(labelPrefab, target);
        label.name = "Label_" + target.name;

        // Position label above the target object
        label.transform.localPosition = Vector3.up * labelHeightOffset;
        label.transform.localRotation = Quaternion.identity;
        label.tag = "LabelSign";
        // Scale up for visibility in AR environment
        label.transform.localScale = new Vector3(100f, 100f, 100f);

        // Create a child GameObject to hold the text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(label.transform, false);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one * 0.01f;

        // Configure TextMeshPro component for 3D text rendering
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();

        // Load custom Montserrat font from Resources
        TMP_FontAsset monsterrateFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/montserratFont");

        // Configure text appearance
        tmp.fontWeight = FontWeight.Bold;
        tmp.text = target.name;  // Display the target object's name
        tmp.font = monsterrateFont;

        // Position text slightly in front of the sign background to prevent z-fighting
        tmp.transform.localPosition = new Vector3(0f, 0f, -0.0005f);
        tmp.transform.localScale = new Vector3(0.004f, 0.004f, 0.01f);

        // Center-align text and enable auto-sizing to fit within bounds
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 2f;
        tmp.fontSizeMax = 65f;
        tmp.color = Color.black;
        tmp.rectTransform.sizeDelta = new Vector2(4f, 4f);

        // Add orbital rotation behavior - makes the label spin around its center
        labelOrbit orbit = label.AddComponent<labelOrbit>();
        orbit.center = target;
        orbit.radius = 0.85f;
        orbit.speed = 40f;  // Rotation speed in degrees per second

        // Add camera-facing behavior - makes label face the player when close
        signLabelLook look = label.AddComponent<signLabelLook>();
    }

    /// <summary>
    /// Destroys all label objects attached to the target transform.
    /// Searches for child objects tagged as "LabelSign" and removes them.
    /// </summary>
    /// <param name="target">Transform to search for labels under</param>
    private void labelDestroy(Transform target)
    {
        // Iterate through all children and destroy any labels
        foreach(Transform child in target)
        {
            if(child.CompareTag("LabelSign"))
                Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Triggered when another collider enters this object's trigger zone.
    /// Creates a label if navigation mode is not active.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone</param>
    public void OnTriggerEnter(Collider other)
    {
        // Only create labels when not in navigation mode
        // This prevents label clutter during active navigation
        if(!parentManager.getnavActive())
            labelCreate(parentObject.transform);
    }

    /// <summary>
    /// Triggered when another collider exits this object's trigger zone.
    /// Destroys the label when the player moves away.
    /// </summary>
    /// <param name="other">The collider that exited the trigger zone</param>
    public void OnTriggerExit(Collider other)
    {
        // Remove label when player leaves the area
        labelDestroy(parentObject.transform);
    }

    /// <summary>
    /// Called once per frame.
    /// Continuously checks navigation state and destroys labels when navigation mode activates.
    /// </summary>
    void Update()
    {
        // Clean up labels when navigation mode is turned on
        // This ensures labels don't interfere with waypoint markers
        if(parentManager.getnavActive())
        {
            labelDestroy(parentObject.transform);
        }
    }
}
