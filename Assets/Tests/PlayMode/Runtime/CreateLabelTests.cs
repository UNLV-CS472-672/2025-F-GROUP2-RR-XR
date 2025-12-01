using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class CreateLabelTests
{
    private GameObject root;
    private createLabel script;

    [SetUp]
    public void Setup()
    {   
        root = new GameObject("Root");
        script = root.AddComponent<createLabel>();


        script.parentObject = new GameObject("Parent");
        var stub = script.parentObject.AddComponent<hideMarkersStub>();
        stub.returnValue = false;

        script.labelPrefab = new GameObject("LabelPrefab");

        script.GetType()
            .GetField("parentManager", 
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .SetValue(script, stub);

        // Create a real XRToggle instance
        var toggleObj = new GameObject("ToggleObj");
        var xr = toggleObj.AddComponent<XRToggle>();
        xr.hideCamera();
        xr.setArMode(true);

        // Inject XRToggle into private field
        script.GetType()
            .GetField("XRToggle",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .SetValue(script, xr);
    }


    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(script.parentObject);
        Object.DestroyImmediate(script.labelPrefab);
    }

    [Test]
    public void labelCreate_creates_label()
    {
        var parent = script.parentObject.transform;

        script.labelCreate(parent);

        Assert.AreEqual(1, parent.childCount);

        var label = parent.GetChild(0);

        Assert.AreEqual("Label_" + parent.name, label.name);
        Assert.AreEqual("LabelSign", label.tag);

        var tmp = label.GetComponentInChildren<TextMeshPro>();
        Assert.IsNotNull(tmp);

        Assert.AreEqual(Vector3.up * script.labelHeightOffset, label.localPosition);
        Assert.AreEqual(parent.name, tmp.text);
    }

    [Test]
    public void labelDestroy_removes_only_label_sign_children()
    {
        var parent = script.parentObject.transform;

        var l1 = new GameObject("Label_One");
        l1.tag = "LabelSign";
        l1.transform.SetParent(parent);

        var l2 = new GameObject("Label_Two");
        l2.tag = "LabelSign";
        l2.transform.SetParent(parent);

        var other = new GameObject("OtherChild");
        other.transform.SetParent(parent);

        script.labelDestroy(parent);

        Assert.AreEqual(1, parent.childCount);
        Assert.AreEqual("OtherChild", parent.GetChild(0).name);
    }

    [Test]
    public void OnTriggerEnter_creates_label_when_AR_mode_and_navFalse()
    {
        var triggerObj = new GameObject("TriggerObj");
        triggerObj.AddComponent<BoxCollider>().isTrigger = true;

        var parent = script.parentObject;
        parent.AddComponent<BoxCollider>();
        parent.AddComponent<Rigidbody>();

        script.OnTriggerEnter(triggerObj.GetComponent<Collider>());

        Assert.AreEqual(1, parent.transform.childCount);

        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void OnTriggerEnter_does_not_create_label_when_nav_true_even_in_AR_mode()
    {
        var triggerObj = new GameObject("TriggerObj");
        triggerObj.AddComponent<BoxCollider>().isTrigger = true;

        var parent = script.parentObject;
        parent.AddComponent<BoxCollider>();
        parent.AddComponent<Rigidbody>();

        var stub = parent.GetComponent<hideMarkersStub>();
        stub.returnValue = true;

        script.OnTriggerEnter(triggerObj.GetComponent<Collider>());

        Assert.AreEqual(0, parent.transform.childCount);

        Object.DestroyImmediate(triggerObj);
    }
}

public class hideMarkersStub : hideMarkers
{
    public bool returnValue = false;
    public override bool getnavActive() => returnValue;
}
