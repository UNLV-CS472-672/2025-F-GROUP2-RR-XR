using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using System.Collections;
using Unity.Collections;
using TMPro;

public class ARQRCodeScanner : MonoBehaviour
{

    [SerializeField] private ARCameraManager arCamManage;
    [SerializeField] private RectTransform roiOverlay;
    [SerializeField] private Button scanButton;
    [SerializeField] private TextMeshProUGUI resultText;
 
    [SerializeField] private IBarcodeReader barcodeReader;
    private Vector2? lastQRCodePos = null; // store result here
    private float lastDecodeTime = 0f;
    public float decodeInterval = 1.0f;

    //this variable will trigger the debugging mode, allowing to check
    //and see the detection of QR code. 
    private bool DEBUGGING = true;
    void Start()
    {
        //This is allowed as it instiantes the Barcode reader
        //and allow user presses
        barcodeReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
        //scanButton.onClick.AddListener(StartScan);
        StartCoroutine(WaitForARSession());
    }

    private IEnumerator WaitForARSession()
    {
        //Standard procedure for ARCamera sesssion usage.
        Debug.Log("Coroutine started");
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;
        }
        Debug.Log("ARSession ready!");
        arCamManage.frameReceived += OnCameraFrameReceived;
    }

     
    void Update()
    {
        if (lastQRCodePos.HasValue)
        {
            // Convert to screen position
            Vector2 screenPos = new Vector2(
                lastQRCodePos.Value.x * Screen.width,
                lastQRCodePos.Value.y * Screen.height
            );

            // Move UI box
            roiOverlay.position = screenPos;
        }
    }
    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {

        if (Time.time - lastDecodeTime < decodeInterval)
            return;
            
        if (!arCamManage.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        lastDecodeTime = Time.time;

        int roiWidth = cpuImage.width / 2;
        int roiHeight = cpuImage.height / 2;
        int roiX = (cpuImage.width - roiWidth) / 2;
        int roiY = (cpuImage.height - roiHeight) / 2;
        // Convert CPU image to byte array (RGBA32)
        // This is for a way to convert camera image into a texture
        // format
        var conversionParams = new XRCpuImage.ConversionParams
        {
            //Creates an image (rectangle)
            inputRect = new RectInt(roiX, roiY, roiWidth, roiHeight),
            //get dimensions
            outputDimensions = new Vector2Int(cpuImage.width/2, cpuImage.height/2),
            //(R, G, B, A) allowing image color channels
            outputFormat = TextureFormat.RGBA32,
        
            transformation = XRCpuImage.Transformation.MirrorY
        };
        //Get the size of the image data
        int size = cpuImage.GetConvertedDataSize(conversionParams);
        //var rawTextureData = new byte[cpuImage.GetConvertedDataSize(conversionParams)];
        using (var rawTextureData = new NativeArray<byte>(size, Allocator.Temp))
        {
            cpuImage.Convert(conversionParams, new NativeSlice<byte>(rawTextureData));
            cpuImage.Dispose();

            Color32[] pixels = new Color32[conversionParams.outputDimensions.x * conversionParams.outputDimensions.y];
            for (int i = 0; i < pixels.Length; i++)
            {
                int idx = i * 4;
                pixels[i] = new Color32(
                    rawTextureData[idx],
                    rawTextureData[idx + 1],
                    rawTextureData[idx + 2],
                    rawTextureData[idx + 3]
                );
            }

            // Optional: Crop to ROI if you want
            // You can calculate roiX, roiY, roiW, roiH based on roiOverlay.rect and camera size

            // Decode QR
            var result = barcodeReader.Decode(pixels, conversionParams.outputDimensions.x, conversionParams.outputDimensions.y);
            if (result != null && result.ResultPoints != null)
            {

                //DEBUGGING PORTION
                if (DEBUGGING)
                {
                    float avgX = 0f;
                    float avgY = 0f;
                    foreach (var p in result.ResultPoints)
                    {
                        avgX += p.X;
                        avgY += p.Y;
                    }
                    avgX /= result.ResultPoints.Length;
                    avgY /= result.ResultPoints.Length;

                    //convert viewpoint then to UI position
                    lastQRCodePos = new Vector2(avgX / conversionParams.outputDimensions.x,
                                                    avgY / conversionParams.outputDimensions.y);

                }
                
                Debug.Log("QR Code Detected: " + result.Text);
                if (resultText != null)
                    resultText.text = "Result: " + result.Text;
            }
            else
            {
                Debug.Log("No QR code detected.");
                if (resultText != null)
                    resultText.text = "No QR code detected.";
            }
        }
    
    }
}
