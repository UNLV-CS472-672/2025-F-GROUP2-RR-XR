using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests_PlayMode")]
//Created by: Alex Yamasaki
public class createLabel : MonoBehaviour
{
    [SerializeField]
    public GameObject parentObject;

    internal bool disableRuntimeBehaviour = false;
    public GameObject labelPrefab;
    private GameObject currentLabel;
    public float labelHeightOffset = 0.3f;
    private hideMarkers parentManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentManager = GetComponentInParent<hideMarkers>();
    }

    //CreateLabel - To create/initialize label object to Unity, 
    internal void labelCreate(Transform target)
    {
        //Create the object
        // that will store the sign(shape)
        // text will be dynamic based on target information
        GameObject label = Instantiate(labelPrefab, target);
        label.name = "Label_" + target.name;

        label.transform.localPosition = Vector3.up * labelHeightOffset;
        label.transform.localRotation = Quaternion.identity;
        label.tag = "LabelSign";
        label.transform.localScale = new Vector3(100f, 100f, 100f);
       

        //label.SetActive(true);
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(label.transform, false);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one * 0.01f;

        //Create text
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        TMP_FontAsset monsterrateFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/montserratFont");
        //if (monsterrateFont == null)
        //    Debug.Log("TEST");

        tmp.fontWeight = FontWeight.Bold;
        tmp.text = target.name;
        tmp.font = monsterrateFont;
        tmp.transform.localPosition = new Vector3(0f, 0f, -0.0005f);
        tmp.transform.localScale = new Vector3(0.004f, 0.004f, 0.01f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 2f;
        tmp.fontSizeMax = 65f;

        tmp.color = Color.black;
        tmp.rectTransform.sizeDelta = new Vector2(4f, 4f);
       
        //rotate the sign around based on the script
        labelOrbit orbit = label.AddComponent<labelOrbit>();
        //variables initialized from the script.
        orbit.center = target;
        orbit.radius = 0.95f;
        orbit.speed = 40f;

        //look at based on player (camera) based on distance.
        signLabelLook look = label.AddComponent<signLabelLook>();
        

    }
<<<<<<< Updated upstream:Assets/Scripts/createLabel.cs
    internal void labelDestroy(Transform target)
=======
    internal void labelDestroy(Transform parent)
>>>>>>> Stashed changes:Assets/Scripts/Runtime/createLabel.cs
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag("LabelSign"))
                DestroyImmediate(child.gameObject);
        }
    }


    public void OnTriggerEnter(Collider other)
    {
<<<<<<< Updated upstream:Assets/Scripts/createLabel.cs
        //Debug.Log(parentManager.getnavActive());
        if(!parentManager.getnavActive())
            labelCreate(parentObject.transform);
=======
        if (disableRuntimeBehaviour) return; 

        if (XRToggle == null || !XRToggle.getARMode())
            return;

        Debug.Log(XRToggle.getARMode());
        Debug.Log(parentManager.getnavActive());
        if(XRToggle.getARMode())
            if(!parentManager.getnavActive())
                labelCreate(parentObject.transform);
>>>>>>> Stashed changes:Assets/Scripts/Runtime/createLabel.cs
    }
    public void OnTriggerExit(Collider other)
    {
        labelDestroy(parentObject.transform);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (disableRuntimeBehaviour) return; 

        if (XRToggle == null) return;

        if(parentManager.getnavActive())
        {
            labelDestroy(parentObject.transform);
        }
    }

    internal void getParent()
    {
        parentManager = GetComponentInParent<hideMarkers>();
    }
}