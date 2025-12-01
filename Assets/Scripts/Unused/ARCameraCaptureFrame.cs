using System.Collections;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
public class ARCameraCaptureFrame : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCamManage;
    [SerializeField] private RectTransform roiOverlay;
    [SerializeField] private Button scanButton;
    private bool isScanning = false;
    void Start()
    {
        StartCoroutine(WaitForARSession());
        scanButton.onClick.AddListener(startScan);
    }

    private IEnumerator WaitForARSession()
    {
        Debug.Log("Coroutine started");
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;
        }
        Debug.Log("ARSession ready!");
        arCamManage.frameReceived += OnCameraFrameReceived;
    }
    void startScan()
    {
        isScanning = true;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        //we would like to scan when its true.
        if (!isScanning) return;
        if (!arCamManage.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        // Convert CPU image to byte array
        byte[] bytes = ConvertCpuImageToByteArray(cpuImage);
        //get CPU image size
        int width = cpuImage.width;
        int height = cpuImage.height;

        //Create a rectangle object based on the given arguments (coordinates)
        //ISSUES: ACKNOLWEDGE HOW ROI WORKS ON APPLE AND UNITY, CONFIGURE IT TO UNITY -> APPLE 
        //        TO MAKE IT WORK.
        Rect normalizedROI = new Rect(0.3f, 0.3f, 0.4f, 0.2f);

        int roiX = (int)(normalizedROI.x * width);
        int roiY = (int)(normalizedROI.y * height);
        int roiW = (int)(normalizedROI.width * width);
        int roiH = (int)(normalizedROI.height * height);
        Debug.Log($"ROI Pixels: X={roiX}, Y={roiY}, W={roiW}, H={roiH}, Img={width}x{height}");

        updateRoiOverlay(normalizedROI);
        // Send bytes to Swift plugin
        visionBridge.RecognizeARFrame(bytes, width, height, roiX, roiY, roiW, roiH);
        //dispose the image
        cpuImage.Dispose();
        //based on the function that ran, store it into a string.
        string recognizedText = visionBridge.getLatestRecognizedText();
        //DEBUGGING
        //Debug.Log("OCR Output (immediate): " + reverse(recognizedText));
        Debug.Log("OCR Output raw: " + recognizedText);

        //set isScanning to false.
        isScanning = false;
    }

    private static byte[] ConvertCpuImageToByteArray(XRCpuImage cpuImage)
    {
        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
            outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
            outputFormat = TextureFormat.RGBA32, 
            transformation = XRCpuImage.Transformation.None
        };

        int size = cpuImage.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        cpuImage.Convert(conversionParams, buffer);

        byte[] bytes = buffer.ToArray();
        buffer.Dispose();

        return bytes;
    }
    //Function: reverse
    //Arguments: str - a string (contains characters)
    //output: Reversed characters from str parameters
    private string reverse(string str)
    {
        string revString = "";
        for (int i = str.Length - 1; i >= 0; i--)
        {
            revString += str[i];
        }
        return revString;
    }
    //NEEDS TO BE FIXED
    //SOME ADJUSTMENTS TOWARDS THE SHAPE
    //This would update the region box.
    private void updateRoiOverlay(Rect normalizedROI)
    {
        if (roiOverlay == null)
            return;
        RectTransform canvasRect = roiOverlay.parent.GetComponent<RectTransform>();
        if (canvasRect == null)
            return;

        float canvasW = canvasRect.rect.width;
        float canvasH = canvasRect.rect.height;

        //generation of object shape
        float uiX = (normalizedROI.x + normalizedROI.width / 2f) * canvasW;
        float uiY = (1f - normalizedROI.y - normalizedROI.height / 2f) * canvasH;

        float uiW = normalizedROI.width * canvasW;
        float uiH = normalizedROI.height * canvasH;

        roiOverlay.pivot = new Vector2(0.5f, 0.5f);
        roiOverlay.anchoredPosition = new Vector2(uiX - canvasW / 2f, uiY - canvasH / 2f);
        roiOverlay.sizeDelta = new Vector2(uiW, uiH);
    }
    //DEBUGGING PURPOSE ONLY
    //void Update() {
    //    
    //if (Time.frameCount % 120 == 0)
    //    { // every ~2 sec
    //        Debug.Log("OCR Output: " + visionBridge.getLatestRecognizedText());
    //    }
    //}
}
