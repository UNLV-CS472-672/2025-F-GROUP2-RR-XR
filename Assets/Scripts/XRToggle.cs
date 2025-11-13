using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
public class XRToggle : MonoBehaviour
{

    //variables
    public List<Button> toggleButtons;
    public GameObject arSessionOrigin;
    public ARSession arSession;
    private bool arModeActive = false;
    public GameObject userUI;
    public GameObject navUI;
    public GameObject immersalSDK;
    public GameObject xrSpace;
   
    [SerializeField]
    private ARCameraBackground arCameraBackground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    

        //when program starts, make camera hidden
        setARMode(false);
        for(int i = 0; i < toggleButtons.Count; i++)
        {
            int index = i;
            toggleButtons[i].onClick.AddListener(toggleARMode);
        }
    }
    void toggleARMode()
    {
        arModeActive = !arModeActive;
        setARMode(arModeActive);
    }

    private void setARMode(bool enable)
    {
   

        if (arCameraBackground != null)
            arCameraBackground.enabled = enable;

        if (userUI != null)
            userUI.SetActive(!enable);
            
        if (navUI != null)
            navUI.SetActive(enable);
 
    }
    // Update is called once per frame
    void Update()
    {

    }
    
}
