using System.Collections;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

/// <summary>
/// Captures AR camera frames and sends them to native iOS Vision API for OCR text recognition.
/// Provides button-triggered frame capture with Region of Interest (ROI) support.
/// </summary>
/// <remarks>
/// This component handles:
/// - AR session initialization and frame event subscription
/// - Button-triggered camera frame capture (not continuous)
/// - Camera frame conversion from XRCpuImage to byte array for native plugin
/// - Region of Interest (ROI) definition and visualization
/// - Integration with visionBridge native iOS plugin for text recognition
/// - UI overlay positioning to show the scanning region
///
/// Technical details:
/// - Uses ARCameraManager.frameReceived event but only captures when button is pressed
/// - Converts RGBA32 camera frames to byte arrays for iOS Vision framework
/// - Sends frame data with ROI coordinates to native Swift/Objective-C plugin
/// - Displays recognized text results via visionBridge
///
/// Known issues:
/// - ROI coordinate system mapping between Unity and iOS needs adjustment
/// - UI overlay positioning may need calibration for different screen sizes
/// </remarks>
public class ARCameraCaptureFrame : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField]
    [Tooltip("AR Camera Manager for accessing camera frames")]
    private ARCameraManager arCamManage;

    [Header("UI References")]
    [SerializeField]
    [Tooltip("Visual overlay showing the Region of Interest for OCR scanning")]
    private RectTransform roiOverlay;

    [SerializeField]
    [Tooltip("Button to trigger frame capture and OCR processing")]
    private Button scanButton;

    /// <summary>
    /// Flag indicating whether a scan is currently in progress.
    /// Set to true when scan button is pressed, reset to false after processing.
    /// </summary>
    private bool isScanning = false;
    /// <summary>
    /// Initializes the AR session wait coroutine and sets up scan button listener.
    /// Called once when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // Wait for AR session to be ready before subscribing to frame events
        StartCoroutine(WaitForARSession());

        // Register button click handler to trigger scanning
        scanButton.onClick.AddListener(startScan);
    }

    /// <summary>
    /// Coroutine that waits for the AR session to reach tracking state.
    /// Once ready, subscribes to camera frame events for OCR processing.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator WaitForARSession()
    {
        Debug.Log("Coroutine started");

        // Wait until AR session is actively tracking
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;  // Wait one frame
        }

        Debug.Log("ARSession ready!");

        // Subscribe to frame events - will only process when isScanning is true
        arCamManage.frameReceived += OnCameraFrameReceived;
    }

    /// <summary>
    /// Initiates a scan by setting the scanning flag.
    /// The next camera frame will be captured and sent for OCR processing.
    /// </summary>
    void startScan()
    {
        isScanning = true;
    }

    /// <summary>
    /// Event handler called when a new AR camera frame is received.
    /// Captures the frame and sends it to native iOS Vision API for OCR when scanning is active.
    /// </summary>
    /// <param name="args">AR camera frame event arguments</param>
    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // Only process frames when scan button has been pressed
        if (!isScanning) return;

        // Attempt to acquire the latest camera frame
        if (!arCamManage.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        // Convert CPU image to byte array for native plugin
        byte[] bytes = ConvertCpuImageToByteArray(cpuImage);

        // Get image dimensions for ROI calculation
        int width = cpuImage.width;
        int height = cpuImage.height;

        // Define Region of Interest in normalized coordinates (0-1 range)
        // Current values: X=30%, Y=30%, Width=40%, Height=20% of frame
        // TODO: ROI coordinate system needs adjustment for Unity -> iOS mapping
        Rect normalizedROI = new Rect(0.3f, 0.3f, 0.4f, 0.2f);

        // Convert normalized ROI to pixel coordinates
        int roiX = (int)(normalizedROI.x * width);
        int roiY = (int)(normalizedROI.y * height);
        int roiW = (int)(normalizedROI.width * width);
        int roiH = (int)(normalizedROI.height * height);

        Debug.Log($"ROI Pixels: X={roiX}, Y={roiY}, W={roiW}, H={roiH}, Img={width}x{height}");

        // Update UI overlay to show the scanning region
        updateRoiOverlay(normalizedROI);

        // Send frame bytes and ROI to native iOS Vision framework for text recognition
        visionBridge.RecognizeARFrame(bytes, width, height, roiX, roiY, roiW, roiH);

        // Dispose of CPU image to free memory
        cpuImage.Dispose();

        // Retrieve recognized text from native plugin
        string recognizedText = visionBridge.getLatestRecognizedText();

        // Log the OCR result
        Debug.Log("OCR Output raw: " + recognizedText);

        // Reset scanning flag - ready for next scan
        isScanning = false;
    }

    /// <summary>
    /// Converts an XRCpuImage to a byte array in RGBA32 format.
    /// Used to prepare camera frames for native iOS plugin processing.
    /// </summary>
    /// <param name="cpuImage">The AR camera image to convert</param>
    /// <returns>Byte array containing RGBA32 pixel data</returns>
    private static byte[] ConvertCpuImageToByteArray(XRCpuImage cpuImage)
    {
        // Configure conversion to RGBA32 format with no transformations
        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
            outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.None
        };

        // Allocate buffer for converted data
        int size = cpuImage.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Convert image to byte array
        cpuImage.Convert(conversionParams, buffer);

        // Copy to managed array and clean up
        byte[] bytes = buffer.ToArray();
        buffer.Dispose();

        return bytes;
    }

    /// <summary>
    /// Reverses a string character by character.
    /// Currently unused - was used for debugging OCR text orientation issues.
    /// </summary>
    /// <param name="str">String to reverse</param>
    /// <returns>Reversed string</returns>
    private string reverse(string str)
    {
        string revString = "";
        for (int i = str.Length - 1; i >= 0; i--)
        {
            revString += str[i];
        }
        return revString;
    }

    /// <summary>
    /// Updates the UI overlay to visually show the Region of Interest on screen.
    /// Converts normalized ROI coordinates to canvas space for display.
    /// TODO: Needs adjustment - overlay positioning may not perfectly match actual ROI
    /// </summary>
    /// <param name="normalizedROI">ROI rectangle in normalized coordinates (0-1 range)</param>
    private void updateRoiOverlay(Rect normalizedROI)
    {
        // Safety checks
        if (roiOverlay == null)
            return;

        RectTransform canvasRect = roiOverlay.parent.GetComponent<RectTransform>();
        if (canvasRect == null)
            return;

        // Get canvas dimensions
        float canvasW = canvasRect.rect.width;
        float canvasH = canvasRect.rect.height;

        // Calculate center position of ROI in canvas space
        // Note: Y coordinate is inverted (1 - y) to convert from bottom-up to top-down
        float uiX = (normalizedROI.x + normalizedROI.width / 2f) * canvasW;
        float uiY = (1f - normalizedROI.y - normalizedROI.height / 2f) * canvasH;

        // Calculate ROI dimensions in canvas space
        float uiW = normalizedROI.width * canvasW;
        float uiH = normalizedROI.height * canvasH;

        // Set overlay transform properties
        roiOverlay.pivot = new Vector2(0.5f, 0.5f);  // Center pivot
        roiOverlay.anchoredPosition = new Vector2(uiX - canvasW / 2f, uiY - canvasH / 2f);
        roiOverlay.sizeDelta = new Vector2(uiW, uiH);
    }

    // DEBUGGING CODE (commented out):
    // Periodically logs OCR output every ~2 seconds
    // void Update()
    // {
    //     if (Time.frameCount % 120 == 0)
    //     {
    //         Debug.Log("OCR Output: " + visionBridge.getLatestRecognizedText());
    //     }
    // }
}
