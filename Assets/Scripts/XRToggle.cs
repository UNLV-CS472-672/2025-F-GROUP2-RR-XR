using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

/// <summary>
/// Controls the AR mode toggle functionality, switching between standard UI and AR camera view.
/// Manages the visibility of AR camera background and related UI elements.
/// </summary>
/// <remarks>
/// This component handles:
/// - Toggle buttons for enabling/disabling AR mode
/// - AR camera background activation/deactivation
/// - Switching between user UI and navigation UI
/// - AR session and XR space management
/// </remarks>
public class XRToggle : MonoBehaviour
{
    [Header("Toggle Buttons")]
    [Tooltip("List of UI buttons that trigger AR mode toggle")]
    public List<Button> toggleButtons;

    [Header("AR Components")]
    [Tooltip("Reference to the AR Session Origin game object")]
    public GameObject arSessionOrigin;

    [Tooltip("Reference to the AR Session component")]
    public ARSession arSession;

    [SerializeField]
    [Tooltip("AR camera background component that renders the device camera feed")]
    private ARCameraBackground arCameraBackground;

    [Tooltip("Immersal SDK game object for visual localization")]
    public GameObject immersalSDK;

    [Tooltip("XR space container for AR content")]
    public GameObject xrSpace;

    [Header("UI Panels")]
    [Tooltip("Standard user interface panel (shown when AR is OFF)")]
    public GameObject userUI;

    [Tooltip("Navigation interface panel (shown when AR is ON)")]
    public GameObject navUI;

    /// <summary>
    /// Current state of AR mode (true = AR active, false = AR inactive)
    /// </summary>
    private bool arModeActive = false;

    /// <summary>
    /// Initializes the AR toggle system and sets up button listeners.
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Start with AR mode disabled - show standard UI
        setARMode(false);

        // Register click handlers for all toggle buttons
        if (toggleButtons != null)
        {
            for (int i = 0; i < toggleButtons.Count; i++)
            {
                // Note: Capture loop variable to avoid closure issues
                int index = i;
                toggleButtons[i].onClick.AddListener(toggleARMode);
            }
        }
    }

    /// <summary>
    /// Toggles AR mode on/off by flipping the current state.
    /// Called when any of the toggle buttons is clicked.
    /// </summary>
    void toggleARMode()
    {
        arModeActive = !arModeActive;
        setARMode(arModeActive);
    }

    /// <summary>
    /// Sets the AR mode to a specific state (enabled or disabled).
    /// Controls visibility of AR camera background and switches between UI panels.
    /// </summary>
    /// <param name="enable">True to enable AR mode, false to disable it</param>
    private void setARMode(bool enable)
    {
        // Enable/disable AR camera background rendering
        if (arCameraBackground != null)
        {
            arCameraBackground.enabled = enable;
        }

        // Toggle UI panels
        // When AR is ON: hide user UI, show navigation UI
        // When AR is OFF: show user UI, hide navigation UI
        if (userUI != null)
        {
            userUI.SetActive(!enable);  // Inverse of AR mode
        }

        if (navUI != null)
        {
            navUI.SetActive(enable);  // Same as AR mode
        }

        // Log state change for debugging
        Debug.Log($"AR Mode: {(enable ? "ENABLED" : "DISABLED")}");
    }

    /// <summary>
    /// Update is called once per frame.
    /// Currently unused but kept for potential future features.
    /// </summary>
    void Update()
    {
        // Reserved for future AR mode updates
        // Potential uses:
        // - Monitor AR session state
        // - Update AR tracking quality indicators
        // - Handle AR mode transitions
    }
}
