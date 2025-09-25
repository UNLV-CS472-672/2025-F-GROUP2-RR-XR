using UnityEngine;

public class OCRDisplay : MonoBehaviour
{
    void Update()
    {
#if UNITY_IOS && !UNITY_EDITOR
        //string result = visionBridge.getText();
        //Debug.Log(result);
        //if(!string.IsNullOrEmpty(result))
        //{
        //    Debug.Log("OCR result: " + result);
        //}
#endif
    }
}
