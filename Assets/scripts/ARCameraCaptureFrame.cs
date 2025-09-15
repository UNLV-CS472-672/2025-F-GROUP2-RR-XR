using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCameraCaptureFrame : MonoBehaviour
{
    [SerializeField] ARCameraManager arCamManage;

    void Start()
    {
        StartCoroutine(WaitForARSession());
    }

    private IEnumerator WaitForARSession()
    {
        Debug.Log("Coroutine started");
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("Waiting for ARSession...");
            yield return null;
        }
        Debug.Log("ARSession ready!");
        arCamManage.frameReceived += OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log("Frame received!");
        // Your frame processing here
    }
}
