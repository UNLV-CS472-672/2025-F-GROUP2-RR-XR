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

        // Mock parent object
        script.parentObject = new GameObject("Parent");

        // Mock prefab
        script.labelPrefab = new GameObject("LabelPrefab");

        // add hideMarkers stub
        var stub = root.AddComponent<hideMarkersStub>();
        stub.returnValue = false;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(script.parentObject);
        Object.DestroyImmediate(script.labelPrefab);
    }

    // ensure labelCreate() creates label
    [Test]
    public void labelCreate_creates_label()
    {
        var parent = script.parentObject.transform;

        script.labelCreate(parent);

        // label should be first child
        Assert.AreEqual(1, parent.childCount);

        var label = parent.GetChild(0);

        Assert.AreEqual("Label_" + parent.name, label.name);
        Assert.AreEqual("LabelSign", label.tag);

        // Verify nested Text object exists
        var tmp = label.GetComponentInChildren<TextMeshPro>();
        Assert.IsNotNull(tmp, "TextMeshPro component should exist");

        // Verify transform setup
        Assert.AreEqual(Vector3.up * script.labelHeightOffset, label.localPosition);
        Assert.AreEqual(Vector3.one * 100f, label.localScale);

        // Text contents
        Assert.AreEqual(parent.name, tmp.text);
    }

    // ensure labelDestroy() removes children
    [Test]
    public void labelDestroy_removes_children()
    {
        var parent = script.parentObject.transform;

        // create two fake labels
        var l1 = new GameObject();
        l1.tag = "LabelSign";
        l1.transform.parent = parent;

        var l2 = new GameObject();
        l2.tag = "LabelSign";
        l2.transform.parent = parent;

        Assert.AreEqual(2, parent.childCount);

        script.labelDestroy(parent);

        // After destroy, Unity still holds destroyed objects in editor
        Assert.AreEqual(0, parent.childCount);
    }

    // ensure OnTriggerEnter creates label when navActive = false
    [Test]
    public void OnTriggerEnter_creates_label_when_nav_false()
    {
        var triggerObj = new GameObject("TriggerObj");
        triggerObj.AddComponent<BoxCollider>().isTrigger = true;

        // Add collider & Rigidbody to parent for triggers
        var parent = script.parentObject;
        parent.AddComponent<BoxCollider>();
        parent.AddComponent<Rigidbody>();

        // Ensure navActive = false
        var stub = root.GetComponent<hideMarkersStub>();
        stub.returnValue = false;

        // Manually call OnTriggerEnter
        script.OnTriggerEnter(triggerObj.GetComponent<Collider>());

        // Test: label should be created
        Assert.AreEqual(1, parent.transform.childCount);

        // Clean up
        Object.DestroyImmediate(triggerObj);
    }

    // ensure OnTriggerEnter does not create label when navActive = true
    [Test]
    public void OnTriggerEnter_does_not_create_label_when_nav_true()
    {
        var triggerObj = new GameObject("TriggerObj");
        triggerObj.AddComponent<BoxCollider>().isTrigger = true;

        var parent = script.parentObject;
        parent.AddComponent<BoxCollider>();
        parent.AddComponent<Rigidbody>();

        // Set navActive = true
        var stub = root.GetComponent<hideMarkersStub>();
        stub.returnValue = true;

        script.OnTriggerEnter(triggerObj.GetComponent<Collider>());

        // Test: no label created
        Assert.AreEqual(0, parent.transform.childCount);

        Object.DestroyImmediate(triggerObj);
    }
}

// ensures getnavActive returns a value
public class hideMarkersStub : MonoBehaviour
{
    public bool returnValue = false;
    public bool getnavActive() => returnValue;
}
