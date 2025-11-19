using System.Runtime.InteropServices;
using UnityEngine;
using System;

/// <summary>
/// Platform bridge for communicating with native iOS Vision framework for text recognition (OCR).
/// Provides Unity-to-iOS interop for processing AR camera frames using Apple's Vision API.
/// </summary>
/// <remarks>
/// This static class handles:
/// - Platform-specific compilation using conditional directives (#if UNITY_IOS)
/// - P/Invoke declarations for calling native iOS Swift/Objective-C code
/// - Marshalling image data from Unity to iOS
/// - Retrieving OCR results from native plugin
/// - Stub implementations for Unity Editor and non-iOS platforms
///
/// Technical details:
/// - Uses DllImport("__Internal") to call iOS plugin methods
/// - Native plugin must be compiled into the Xcode project
/// - Image data is passed as byte array with dimensions and ROI parameters
/// - Returns text results as managed string
///
/// Platform support:
/// - iOS: Full functionality with native Vision framework
/// - Unity Editor: Returns empty string (stub implementation)
/// - Android/Other: Returns empty string (stub implementation)
/// </remarks>
public static class visionBridge
{
#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// Retrieves the most recently recognized text from the native iOS Vision plugin.
    /// Called after RecognizeARFrame to get OCR results.
    /// </summary>
    /// <returns>Recognized text string from the last processed frame, or empty string if none found</returns>
    [DllImport("__Internal")]
    public static extern string getLatestRecognizedText();

    /// <summary>
    /// Native iOS method that processes image bytes using Vision framework for text recognition.
    /// Private - use RecognizeARFrame wrapper instead.
    /// </summary>
    /// <param name="imageData">RGBA32 image data as byte array</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="roiX">Region of Interest X coordinate in pixels</param>
    /// <param name="roiY">Region of Interest Y coordinate in pixels</param>
    /// <param name="roiW">Region of Interest width in pixels</param>
    /// <param name="roiH">Region of Interest height in pixels</param>
    [DllImport("__Internal")]
    private static extern void recognizeTextFromBytes(byte[] imageData, int width, int height,
                                            int roiX, int roiY, int roiW, int roiH);

    /// <summary>
    /// Public wrapper for sending AR camera frames to the native iOS Vision plugin for OCR.
    /// Validates input and calls the native recognizeTextFromBytes method.
    /// </summary>
    /// <param name="imageData">RGBA32 image data as byte array from AR camera</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="roiX">Region of Interest X coordinate in pixels</param>
    /// <param name="roiY">Region of Interest Y coordinate in pixels</param>
    /// <param name="roiW">Region of Interest width in pixels</param>
    /// <param name="roiH">Region of Interest height in pixels</param>
    public static void RecognizeARFrame(byte[] imageData, int width, int height,
                                        int roiX, int roiY, int roiW, int roiH)
    {
        // Validate input data before sending to native code
        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogWarning("RecognizeARFrame called with empty image data.");
            return;
        }

        // Forward to native iOS Vision framework
        recognizeTextFromBytes(imageData, width, height, roiX, roiY, roiW, roiH);
    }
#else
    /// <summary>
    /// Stub implementation for Unity Editor and non-iOS platforms.
    /// Returns empty string since native iOS Vision framework is not available.
    /// </summary>
    /// <returns>Empty string</returns>
    public static string getLatestRecognizedText() { return ""; }

    /// <summary>
    /// Stub implementation for Unity Editor and non-iOS platforms.
    /// Does nothing since native iOS Vision framework is not available.
    /// </summary>
    /// <param name="imageData">Ignored in stub implementation</param>
    /// <param name="width">Ignored in stub implementation</param>
    /// <param name="height">Ignored in stub implementation</param>
    /// <param name="roiX">Ignored in stub implementation</param>
    /// <param name="roiY">Ignored in stub implementation</param>
    /// <param name="roiW">Ignored in stub implementation</param>
    /// <param name="roiH">Ignored in stub implementation</param>
    public static void RecognizeARFrame(byte[] imageData, int width, int height,
                                        int roiX, int roiY, int roiW, int roiH) { }
#endif
}
