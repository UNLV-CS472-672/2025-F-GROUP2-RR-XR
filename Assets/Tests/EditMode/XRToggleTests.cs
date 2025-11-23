using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class XRToggleTests
{
    private GameObject root;
    private XRToggle xrToggle;

    private GameObject userUI;
    private GameObject navUI;
    private GameObject bottomButtons;
    private GameObject searchUI;
    private ARCameraBackground arCamBackground;

    [SetUp]
    public void Setup()
    {
        root = new GameObject("XRToggleRoot");
        xrToggle = root.AddComponent<XRToggle>();

        // Create UI objects
        userUI = new GameObject("UserUI");
        userUI.transform.parent = root.transform;

        navUI = new GameObject("NavUI");
        navUI.transform.parent = root.transform;

        bottomButtons = new GameObject("BottomButtons");
        bottomButtons.transform.parent = root.transform;

        searchUI = new GameObject("SearchUI");
        searchUI.transform.parent = root.transform;

        // Add child objects to test setChildrenActive
        for (int i = 0; i < 2; i++)
        {
            var child = new GameObject("Child" + i);
            child.transform.parent = searchUI.transform;
        }

        // Add AR camera background
        var arCamObj = new GameObject("ARCamera");
        arCamBackground = arCamObj.AddComponent<ARCameraBackground>();

        // Assign references
        xrToggle.userUI = userUI;
        xrToggle.navUI = navUI;
        xrToggle.bottomButtons = bottomButtons;
        xrToggle.searchUI = searchUI;
        xrToggle.arCameraBackground = arCamBackground;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(userUI);
        Object.DestroyImmediate(navUI);
        Object.DestroyImmediate(bottomButtons);
        Object.DestroyImmediate(searchUI);
        Object.DestroyImmediate(arCamBackground.gameObject);
    }

    // ensure toggleARMode switches ui
    [Test]
    public void toggleARMode_switches_ui()
    {
        xrToggle.setARMode(false);

        xrToggle.toggleARMode(); // Enable AR
        Assert.IsTrue(arCamBackground.enabled);
        Assert.IsFalse(userUI.activeSelf);
        Assert.IsFalse(navUI.activeSelf);

        xrToggle.toggleARMode(); // Disable AR
        Assert.IsFalse(arCamBackground.enabled);
        Assert.IsTrue(userUI.activeSelf);
        Assert.IsTrue(navUI.activeSelf);
    }

    // ensure enableNavigationMode hides ui
    [Test]
    public void enableNavigationMode_hides_ui()
    {
        xrToggle.enableNavigationMode();

        Assert.IsFalse(userUI.activeSelf);
        Assert.IsTrue(bottomButtons.activeSelf);

        foreach (Transform child in searchUI.transform)
            Assert.IsFalse(child.gameObject.activeSelf);
    }

    // ensure disableNavigationMode shows ui
    [Test]
    public void DisableNavigationMode_shows_ui()
    {
        xrToggle.DisableNavigationMode();

        Assert.IsTrue(userUI.activeSelf);
        Assert.IsFalse(bottomButtons.activeSelf);

        foreach (Transform child in navUI.transform)
            Assert.IsTrue(child.gameObject.activeSelf);
    }

    // ensure setChildrenActive sets children correctly
    [Test]
    public void setChildrenActive_sets_children()
    {
        var ignoredChild = new GameObject("Ignored");
        ignoredChild.tag = "ignore";
        ignoredChild.transform.parent = bottomButtons.transform;

        xrToggle.setChildrenActive(bottomButtons, true);

        foreach (Transform child in bottomButtons.transform)
        {
            if (child.CompareTag("ignore"))
                Assert.IsFalse(child.gameObject.activeSelf);
            else
                Assert.IsTrue(child.gameObject.activeSelf);
        }
    }

    // ensure setARMode disables ui when on
    [Test]
    public void setARMode_disables_UI()
    {
        xrToggle.setARMode(true);

        Assert.IsTrue(arCamBackground.enabled);
        Assert.IsFalse(userUI.activeSelf);
        Assert.IsFalse(navUI.activeSelf);
    }

    // ensure setARMode enables ui when off
    [Test]
    public void setARMode_enables_UI()
    {
        xrToggle.setARMode(false);

        Assert.IsFalse(arCamBackground.enabled);
        Assert.IsTrue(userUI.activeSelf);
        Assert.IsTrue(navUI.activeSelf);
    }
}
