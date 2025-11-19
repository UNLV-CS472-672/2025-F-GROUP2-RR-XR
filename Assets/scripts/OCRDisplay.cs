using UnityEngine;

/// <summary>
/// DEBUG UTILITY: Displays OCR results from the visionBridge native iOS plugin.
/// Currently inactive - all functionality has been commented out.
/// </summary>
/// <remarks>
/// This component was intended to:
/// - Continuously poll visionBridge for recognized text
/// - Display OCR results in the console for debugging
/// - Only function on iOS builds (not in Unity Editor)
///
/// Current status: All code is commented out. The functionality has likely been
/// moved to ARCameraCaptureFrame.cs which handles OCR display more directly.
///
/// TODO: Either implement active OCR display functionality or remove this file
/// if it's no longer needed.
/// </remarks>
public class OCRDisplay : MonoBehaviour
{
    /// <summary>
    /// Called once per frame.
    /// Currently empty - all OCR polling code is commented out.
    /// </summary>
    void Update()
    {
#if UNITY_IOS && !UNITY_EDITOR
        // ORIGINAL DEBUG CODE (commented out):
        // Polls visionBridge for text recognition results
        // and logs them to the console

        // string result = visionBridge.getText();
        // Debug.Log(result);
        // if(!string.IsNullOrEmpty(result))
        // {
        //     Debug.Log("OCR result: " + result);
        // }
#endif
    }
}
