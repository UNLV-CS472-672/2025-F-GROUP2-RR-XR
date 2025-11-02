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
        //Create the object
        // that will store the sign(shape)
        // text will be dynamic based on target information
        GameObject label = Instantiate(labelPrefab, target);
        label.name = "Label_" + target.name;

        label.transform.localPosition = Vector3.up * labelHeightOffset;
        label.transform.localRotation = Quaternion.identity;
        label.tag = "LabelSign";
        label.transform.localScale = new Vector3(0.6f, 0.485f, 0.01318f);
       

        //label.SetActive(true);
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(label.transform, false);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one * 0.01f;

        //Create text
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = target.name;
        tmp.transform.localPosition = new Vector3(0f, 0f, -0.65f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 18f;
        tmp.fontSizeMax = 150f;

        tmp.color = Color.black;
        tmp.rectTransform.sizeDelta = new Vector2(100f, 100f);

        //rotate the sign around based on the script
        labelOrbit orbit = label.AddComponent<labelOrbit>();
        //variables initialized from the script.
        orbit.center = target;
        orbit.radius = 0.3f;
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

    }
}
