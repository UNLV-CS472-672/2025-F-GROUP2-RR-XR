using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro;
public class XRToggle : MonoBehaviour
{

    //variables
    private bool arModeActive = false;
    [SerializeField]
    public GameObject userUI;
    [SerializeField]
    public GameObject navUI;
    [SerializeField]
    private GameObject background;
    [SerializeField]
    public GameObject searchUI;
    [SerializeField] 
    private TMP_InputField searchBox;
    [SerializeField]
    public GameObject bottomButtons;
 
    public static XRToggle Instance;
    [SerializeField]
    private GameObject searchVarient;

    [SerializeField]
    private ARCameraBackground arCameraBackground;

    //THIS SCRIPT TOOK ME HOURS TO MAKE AND THINK ABOUT
    //NEVER IN MY LIFE I HAVE SPENT SOO MUCH TIME DEBUGGING CODE
    //WITH THIS SIMPLE TOGGLE OFF AND ON SWITCH.

    //I HAD TROUBLE COMPHRENDING AND GOT CONFUSED OF HOW IT WORKS
    //BUT IT WORKS. WITH SIMPLE LOGIC AND BREAKING IT DOWN SLOWLY INVOLVED
    //SOLVING THIS PROBLEM.
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
        //Debug.LogWarning("Duplicate XRToggle detected! Destroying extra copy.");
            Destroy(gameObject);
            return;
        }

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

        //This will empty the search box once user enters UI mode. 
        if (searchBox != null)
            searchBox.text = string.Empty;
        arModeActive = false;
        userUI.SetActive(false);
        setChildrenActiveFilter(searchUI, false);
        setChildrenActiveFilter(bottomButtons, true);
        setChildrenActiveFilter(searchVarient, false);


    }
    //This function will change to the UI to user UI mode
    public void DisableNavigationMode()
    {
    
        arModeActive = false;
        setChildrenActiveFilter(navUI, true);
        setChildrenActiveFilter(bottomButtons, false);
        Transform child = searchVarient.transform.Find("MainMenuBackground");
        if(child != null)
            child.gameObject.SetActive(true);
        userUI.SetActive(true);
        
        
    }
    //This function will set to userUI when program starts
    public void toggleARMode()
    {
        arModeActive = !arModeActive;
        setARMode(arModeActive);
    }
    public void setArMode(bool enable)
    {
        arModeActive = enable;
    }
    public bool getARMode()
    {
        return arModeActive;
    }
    public void setARMode(bool enable)
    {
        //if (arCameraBackground != null)
        //    arCameraBackground.enabled = enable;
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
    public void setChildrenActiveFilter(GameObject parent, bool state)
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
    
}
