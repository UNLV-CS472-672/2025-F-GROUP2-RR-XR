using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HideMarkersTests
{
    private GameObject parentObj;
    private hideMarkers script;

    [SetUp]
    public void Setup()
    {
        // Create a parent with two children, each with two subchildren

        parentObj = new GameObject("ParentObj");

        GameObject childA = new GameObject("ChildA");
        childA.transform.SetParent(parentObj.transform);

        new GameObject("SubA1").transform.SetParent(childA.transform);
        new GameObject("SubA2").transform.SetParent(childA.transform);

        GameObject childB = new GameObject("ChildB");
        childB.transform.SetParent(parentObj.transform);

        new GameObject("SubB1").transform.SetParent(childB.transform);
        new GameObject("SubB2").transform.SetParent(childB.transform);

        GameObject holder = new GameObject("ScriptHolder");
        script = holder.AddComponent<hideMarkers>();

        // Assign parentObj via serialized field
        typeof(hideMarkers)
            .GetField("parentObj", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, parentObj);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(parentObj);
        Object.DestroyImmediate(script.gameObject);
    }

    // ensure toggleNavActive works
    [Test]
    public void toggleNavActive_test()
    {
        Assert.IsFalse(script.getnavActive(), "Initially navActive should be false");

        script.toggleNavActive();
        Assert.IsTrue(script.getnavActive());

        script.toggleNavActive();
        Assert.IsFalse(script.getnavActive());
    }

    // ensure hideMarkersVisual hides subchildren
    [Test]
    public void hideMarkersVisual_hides_subchildren()
    {
        // ensure all active before hiding
        foreach (Transform child in parentObj.transform)
            foreach (Transform sub in child)
                sub.gameObject.SetActive(true);

        script.hideMarkersVisual();

        foreach (Transform child in parentObj.transform)
        {
            foreach (Transform sub in child)
            {
                Assert.IsFalse(sub.gameObject.activeSelf, $"{sub.name} should be inactive");
            }
        }
    }

    // ensure showMarker activates only the target
    [Test]
    public void showMarker_activates_only_target()
    {
        Transform target = parentObj.transform.GetChild(0); // ChildA

        // ensure all are active before running
        foreach (Transform child in parentObj.transform)
            foreach (Transform sub in child)
                sub.gameObject.SetActive(true);

        script.showMarker(target, navOption: false);

        // Verify target children are active
        foreach (Transform sub in target)
        {
            Assert.IsTrue(sub.gameObject.activeSelf, $"{sub.name} should be active");
        }

        // Verify children of non-target parent are inactive
        Transform other = parentObj.transform.GetChild(1); // ChildB
        foreach (Transform sub in other)
        {
            Assert.IsFalse(sub.gameObject.activeSelf, $"{sub.name} should be inactive");
        }
    }

    // ensure showMarker toggles navActive when navOption = true
    [Test]
    public void showMarker_toggles_navActive()
    {
        Assert.IsFalse(script.getnavActive());

        Transform target = parentObj.transform.GetChild(0);
        script.showMarker(target, navOption: true);

        Assert.IsTrue(script.getnavActive(), "showMarker should toggle navActive when option is true");
    }

    
    // ensure showMarker does not toggle navActive when navOption = false
    [Test]
    public void showMarker_doesnt_toggle_navActive()
    {
        Assert.IsFalse(script.getnavActive());

        Transform target = parentObj.transform.GetChild(0);
        script.showMarker(target, navOption: false);

        Assert.IsFalse(script.getnavActive(), "showMarker should not toggle navActive when option is false");
    }

}
