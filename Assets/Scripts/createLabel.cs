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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    //CreateLabel - To create/initialize label object to Unity, 
    private void labelCreate(Transform target)
    {
        //Create the object
        GameObject label = Instantiate(labelPrefab, target);
        label.name = "Label_" + target.name;
        label.transform.localPosition = Vector3.up * labelHeightOffset;

        //label.SetActive(true);
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(label.transform, false);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localRotation = Quaternion.identity;
        textObj.transform.localScale = Vector3.one * 0.01f;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = target.name;
        tmp.transform.localPosition = new Vector3(0f, 0f, -0.5f);
        tmp.fontSize = 1;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;

        tmp.color = Color.black;
        tmp.rectTransform.sizeDelta = new Vector2(100f, 100f);

    }
    private void labelDestroy(Transform target)
    {
        Transform labelChild = target.Find("Label_ " + target.name);
    }
    public void OnTriggerEnter()
    {
        labelCreate(parentObject.transform);
    }
    public void OnTriggerExit()
    {
        labelDestroy(parentObject.transform);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
