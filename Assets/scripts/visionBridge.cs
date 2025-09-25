using System.Runtime.InteropServices;
using UnityEngine;
using System;
public static class visionBridge
{
#if UNITY_IOS && !UNITY_EDITOR
    // Method to get latest recognized text from native iOS plugin
    [DllImport("__Internal")]
    public static extern string getLatestRecognizedText();

    // Method to send AR frame bytes to Swift/Obj-C plugin
    [DllImport("__Internal")]
    private static extern void recognizeTextFromBytes(byte[] imageData, int width, int height, 
                                            int roiX, int roiY, int roiW, int roiH);

    // Public wrapper for Unity scripts
    public static void RecognizeARFrame(byte[] imageData, int width, int height,
                                        int roiX, int roiY, int roiW, int roiH)
    {
        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogWarning("RecognizeARFrame called with empty image data.");
            return;
        }

        recognizeTextFromBytes(imageData, width, height, roiX, roiY, roiW, roiH);
    }
#else
    // Stub implementations for editor or non-iOS platforms
    public static string getLatestRecognizedText() { return ""; }
    public static void RecognizeARFrame(byte[] imageData, int width, int height,
                                        int roiX, int roiY, int roiW, int roiH) { }
#endif
}
