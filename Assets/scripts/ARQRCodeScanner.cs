using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using System.Collections;
using Unity.Collections;
using TMPro;

/// <summary>
/// Scans QR codes in real-time using the AR camera feed and ZXing barcode library.
/// Provides continuous scanning with a configurable Region of Interest (ROI).
/// </summary>
/// <remarks>
/// This component handles:
/// - AR session initialization and camera frame event handling
/// - Region of Interest (ROI) based scanning to improve performance
/// - Real-time QR code detection and decoding using ZXing library
/// - Visual feedback with UI overlay showing detected QR code position
/// - Throttled scanning with configurable decode interval
/// - Camera frame conversion from XRCpuImage to Color32 array for ZXing
///
/// Technical details:
/// - Uses ARCameraManager.frameReceived event for continuous scanning
/// - Converts RGBA32 camera frames to pixel array for barcode reading
/// - Implements auto-rotation and inverted QR code detection
/// - Displays detection results in UI text field
/// </remarks>
public class ARQRCodeScanner : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField]
    [Tooltip("AR Camera Manager for accessing camera frames")]
    private ARCameraManager arCamManage;

    [Header("UI References")]
    [SerializeField]
    [Tooltip("Visual overlay showing the Region of Interest for QR scanning")]
    private RectTransform roiOverlay;

    [SerializeField]
    [Tooltip("Button to manually trigger scanning (currently unused - scanning is automatic)")]
    private Button scanButton;

    [SerializeField]
    [Tooltip("Text field for displaying QR code scan results")]
    private TextMeshProUGUI resultText;

    [Header("Barcode Configuration")]
    [SerializeField]
    [Tooltip("ZXing barcode reader instance (configured for auto-rotation and inverted codes)")]
    private IBarcodeReader barcodeReader;

    [Tooltip("Time interval between decode attempts in seconds (default: 1.0s)")]
    public float decodeInterval = 1.0f;

    /// <summary>
    /// Stores the normalized screen position of the last detected QR code.
    /// Used for positioning the visual overlay. Null if no code detected yet.
    /// </summary>
    private Vector2? lastQRCodePos = null;

    /// <summary>
    /// Timestamp of the last decode attempt.
    /// Used to throttle scanning frequency based on decodeInterval.
    /// </summary>
    private float lastDecodeTime = 0f;

    /// <summary>
    /// Enables debug visualization mode.
    /// When true, shows a UI overlay at the detected QR code position.
    /// </summary>
    private bool DEBUGGING = true;
    /// <summary>
    /// Initializes the barcode reader and waits for AR session to be ready.
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Initialize ZXing barcode reader with configuration
        // AutoRotate: Handles QR codes at any orientation
        // TryInverted: Can read white-on-black QR codes in addition to black-on-white
        barcodeReader = new BarcodeReader { AutoRotate = true, TryInverted = true };

        // Note: Scan button listener commented out - currently using automatic continuous scanning
        // scanButton.onClick.AddListener(StartScan);

        // Wait for AR session to initialize before subscribing to camera frames
        StartCoroutine(WaitForARSession());
    }

    /// <summary>
    /// Coroutine that waits for the AR session to reach tracking state.
    /// Once ready, subscribes to camera frame events to begin QR scanning.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator WaitForARSession()
    {
        // Standard procedure for AR camera session initialization
        Debug.Log("Coroutine started");

        // Wait until AR session is actively tracking
        // This ensures the camera is ready to provide frames
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;  // Wait one frame and check again
        }

        Debug.Log("ARSession ready!");

        // Subscribe to frame received event to process each camera frame
        arCamManage.frameReceived += OnCameraFrameReceived;
    }

    /// <summary>
    /// Called once per frame.
    /// Updates the ROI overlay position based on the last detected QR code location.
    /// </summary>
    void Update()
    {
        // Only update overlay if a QR code has been detected
        if (lastQRCodePos.HasValue)
        {
            // Convert normalized position (0-1) to screen pixel coordinates
            Vector2 screenPos = new Vector2(
                lastQRCodePos.Value.x * Screen.width,
                lastQRCodePos.Value.y * Screen.height
            );

            // Position the UI overlay at the detected QR code location
            roiOverlay.position = screenPos;
        }
    }
    /// <summary>
    /// Event handler called when a new AR camera frame is received.
    /// Processes the frame to detect and decode QR codes within the Region of Interest.
    /// </summary>
    /// <param name="args">AR camera frame event arguments</param>
    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // Throttle scanning to prevent excessive processing
        // Only attempt decode after the specified interval has passed
        if (Time.time - lastDecodeTime < decodeInterval)
            return;

        // Attempt to acquire the latest camera frame as a CPU image
        // Returns false if no frame is available
        if (!arCamManage.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        // Update timestamp for throttling
        lastDecodeTime = Time.time;

        // Define Region of Interest (ROI) as center half of the image
        // This improves performance by scanning only the central area
        int roiWidth = cpuImage.width / 2;
        int roiHeight = cpuImage.height / 2;
        int roiX = (cpuImage.width - roiWidth) / 2;
        int roiY = (cpuImage.height - roiHeight) / 2;

        // Configure conversion parameters to transform XRCpuImage to texture format
        // This converts the raw camera data into a format ZXing can process
        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Define the rectangular region to extract (ROI)
            inputRect = new RectInt(roiX, roiY, roiWidth, roiHeight),

            // Downscale to half resolution for faster processing
            outputDimensions = new Vector2Int(cpuImage.width/2, cpuImage.height/2),

            // RGBA32 format provides 4 bytes per pixel (Red, Green, Blue, Alpha)
            outputFormat = TextureFormat.RGBA32,

            // Mirror Y axis to correct camera orientation
            transformation = XRCpuImage.Transformation.MirrorY
        };

        // Calculate required buffer size for converted image data
        int size = cpuImage.GetConvertedDataSize(conversionParams);

        // Use native array for efficient memory management (auto-disposed with 'using')
        using (var rawTextureData = new NativeArray<byte>(size, Allocator.Temp))
        {
            // Convert the XRCpuImage to RGBA32 byte array
            cpuImage.Convert(conversionParams, new NativeSlice<byte>(rawTextureData));

            // Dispose of the CPU image immediately after conversion to free memory
            cpuImage.Dispose();

            // Convert byte array to Color32 array for ZXing barcode reader
            Color32[] pixels = new Color32[conversionParams.outputDimensions.x * conversionParams.outputDimensions.y];
            for (int i = 0; i < pixels.Length; i++)
            {
                // Each pixel is 4 consecutive bytes (RGBA)
                int idx = i * 4;
                pixels[i] = new Color32(
                    rawTextureData[idx],      // Red
                    rawTextureData[idx + 1],  // Green
                    rawTextureData[idx + 2],  // Blue
                    rawTextureData[idx + 3]   // Alpha
                );
            }

            // Attempt to decode QR code from the pixel array
            var result = barcodeReader.Decode(pixels, conversionParams.outputDimensions.x, conversionParams.outputDimensions.y);

            // Check if a QR code was successfully detected
            if (result != null && result.ResultPoints != null)
            {
                // DEBUG MODE: Calculate and store QR code position for visual overlay
                if (DEBUGGING)
                {
                    // Calculate average position of all QR code corner points
                    float avgX = 0f;
                    float avgY = 0f;
                    foreach (var p in result.ResultPoints)
                    {
                        avgX += p.X;
                        avgY += p.Y;
                    }
                    avgX /= result.ResultPoints.Length;
                    avgY /= result.ResultPoints.Length;

                    // Normalize position to 0-1 range for screen-space conversion
                    lastQRCodePos = new Vector2(avgX / conversionParams.outputDimensions.x,
                                                    avgY / conversionParams.outputDimensions.y);
                }

                // Log and display the decoded QR code text
                Debug.Log("QR Code Detected: " + result.Text);
                if (resultText != null)
                    resultText.text = "Result: " + result.Text;
            }
            else
            {
                // No QR code found in this frame
                Debug.Log("No QR code detected.");
                if (resultText != null)
                    resultText.text = "No QR code detected.";
            }
        }
    }
}
