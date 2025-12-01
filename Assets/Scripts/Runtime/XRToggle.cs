using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
<<<<<<< Updated upstream:Assets/Scripts/XRToggle.cs
=======
using TMPro;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests_PlayMode")]
>>>>>>> Stashed changes:Assets/Scripts/Runtime/XRToggle.cs
public class XRToggle : MonoBehaviour
{
    //variables
       private bool arModeActive = false;
    public GameObject userUI;
    public GameObject navUI;

    public GameObject searchUI;

    public GameObject bottomButtons;
    public GameObject immersalSDK;
    public GameObject xrSpace;
    public static XRToggle Instance;

<<<<<<< Updated upstream:Assets/Scripts/XRToggle.cs
    [SerializeField]
    internal ARCameraBackground arCameraBackground;
=======
    //[SerializeField]
    //private ARCameraBackground arCameraBackground;
>>>>>>> Stashed changes:Assets/Scripts/Runtime/XRToggle.cs

    //THIS SCRIPT TOOK ME HOURS TO MAKE AND THINK ABOUT
    //NEVER IN MY LIFE I HAVE SPENT SOO MUCH TIME DEBUGGING CODE
    //WITH THIS SIMPLE TOGGLE OFF AND ON SWITCH.

    //I HAD TROUBLE COMPHRENDING AND GOT CONFUSED OF HOW IT WORKS
    //BUT IT WORKS. WITH SIMPLE LOGIC AND BREAKING IT DOWN SLOWLY INVOLVED
    //SOLVING THIS PROBLEM.
    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //when program starts, make camera hidden
        setARMode(false);
 
    }
    //This function will change the UI to navigation Mode
    public void enableNavigationMode()
    {
        //DEBUGGING
        //BRUTE FORCING METHOD
        //WHEN USER PRESSES BUTTON IT WILL REMOVE USER UI
        //AND MAKE SEARCHBAR APPEAR AND MAKE THE HOMESCREEN BOTTOM BUTTON APPEAR
        arModeActive = true;
        userUI.SetActive(false);
        setChildrenActive(searchUI, false);
        setChildrenActive(bottomButtons, true);


    }
    //This function will change to the UI to user UI mode
    public void DisableNavigationMode()
    {
    
        arModeActive = false;
        setChildrenActive(navUI, true);
        setChildrenActive(bottomButtons, false);
        userUI.SetActive(true);
    }
    //This function will set to userUI when program starts
    public void toggleARMode()
    {
<<<<<<< Updated upstream:Assets/Scripts/XRToggle.cs
         
=======
        if (searchVarient != null)
        {
            Transform child = searchVarient.transform.Find("MainMenuBackground");
            if (child != null)
                child.gameObject.SetActive(arModeActive);
        }
        
>>>>>>> Stashed changes:Assets/Scripts/Runtime/XRToggle.cs
        arModeActive = !arModeActive;
        setARMode(arModeActive);
    }

    public void setARMode(bool enable)
    {
     
        if (arCameraBackground != null)
            arCameraBackground.enabled = enable;
        //will make the screen appear when startup
        if (userUI != null)
        {
            userUI.SetActive(!enable);
            //setChildrenActive(navUI, true);
        }
        if (navUI != null)
        {
            navUI.SetActive(!enable);
        }

       
    }
    //Void function that will set child objects appear/disappear
    public void setChildrenActive(GameObject parent, bool state)
    {
        
        if (parent == null)
            return;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            //if certain objects doesn't have tag ignore
            //then it will hide
            if (!parent.transform.GetChild(i).CompareTag("ignore"))
            {
              
                Transform childTransform = parent.transform.GetChild(i);
                childTransform.gameObject.SetActive(state);
            }
        
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    //testing method to imitate start()
    internal void hideCamera()
    {
        setARMode(false);
    }
    
}
