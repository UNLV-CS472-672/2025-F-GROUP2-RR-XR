using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Created by: Alex Yamasaki
public class createLabel : MonoBehaviour
{
    [SerializeField]
    public GameObject parentObject;


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
    private void labelCreate(Transform target)
    {
        Debug.Log("SDFLKJSFD");
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
    private void labelDestroy(Transform target)
    {
        foreach(Transform child in target)
        {
            if(child.CompareTag("LabelSign"))
                Destroy(child.gameObject);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(parentManager.getnavActive());
        if(!parentManager.getnavActive())
            labelCreate(parentObject.transform);
    }
    public void OnTriggerExit(Collider other)
    {
        labelDestroy(parentObject.transform);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(parentManager.getnavActive())
        {
            labelDestroy(parentObject.transform);
        }
    }
}
