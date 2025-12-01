using NUnit.Framework;
using UnityEngine;

public class XRToggleTests
{
    private GameObject root;
    private XRToggle xrToggle;

    private GameObject userUI;
    private GameObject navUI;
    private GameObject bottomButtons;
    private GameObject searchUI;
    private GameObject searchVarient;

    [SetUp]
    public void Setup()
    {
        root = new GameObject("XRToggleRoot");
        xrToggle = root.AddComponent<XRToggle>();

        // Required objects
        userUI = new GameObject("UserUI");
        navUI = new GameObject("NavUI");
        bottomButtons = new GameObject("BottomButtons");
        searchUI = new GameObject("SearchUI");
        searchVarient = new GameObject("SearchVarient");

        // searchVarient MUST contain "MainMenuBackground"
        var mainMenuBG = new GameObject("MainMenuBackground");
        mainMenuBG.transform.parent = searchVarient.transform;

        // Attach them
        xrToggle.userUI = userUI;
        xrToggle.navUI = navUI;
        xrToggle.bottomButtons = bottomButtons;
        xrToggle.searchUI = searchUI;

        // inject searchVarient into private serialized field
        typeof(XRToggle)
            .GetField("searchVarient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(xrToggle, searchVarient);

        // Add a few children to searchUI so setChildrenActiveFilter can be tested meaningfully
        for (int i = 0; i < 2; i++)
        {
            new GameObject("Child" + i).transform.parent = searchUI.transform;
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
    }

    [Test]
    public void toggleARMode_switches_ui_correctly()
    {
        xrToggle.hideCamera();

        // enable AR
        xrToggle.toggleARMode();
        Assert.IsFalse(userUI.activeSelf);
        Assert.IsFalse(navUI.activeSelf);

        // disable AR
        xrToggle.toggleARMode();
        Assert.IsTrue(userUI.activeSelf);
        Assert.IsTrue(navUI.activeSelf);
    }

    [Test]
    public void enableNavigationMode_hides_userUI_and_shows_bottomButtons()
    {
        xrToggle.enableNavigationMode();

        Assert.IsFalse(userUI.activeSelf);
        Assert.IsTrue(bottomButtons.activeSelf);

        foreach (Transform child in searchUI.transform)
            Assert.IsFalse(child.gameObject.activeSelf);
    }

    [Test]
    public void disableNavigationMode_shows_userUI_and_hides_bottomButtons()
    {
        xrToggle.DisableNavigationMode();

        Assert.IsTrue(userUI.activeSelf);
        foreach (Transform child in bottomButtons.transform)
            Assert.IsFalse(child.gameObject.activeSelf);

    }

    [Test]
    public void setChildrenActiveFilter_respects_ignore_tag()
    {
        var ignoredChild = new GameObject("Ignored");
        ignoredChild.tag = "ignore";
        ignoredChild.transform.parent = bottomButtons.transform;

        // Add one normal child
        var normalChild = new GameObject("Normal");
        normalChild.transform.parent = bottomButtons.transform;

        xrToggle.setChildrenActiveFilter(bottomButtons, true);

        // "ignore" children remain unchanged (default = active)
        Assert.IsTrue(ignoredChild.activeSelf);

        // normal children follow state
        Assert.IsTrue(normalChild.activeSelf);
    }

    [Test]
    public void setARMode_disables_UI_when_true()
    {
        xrToggle.setARMode(true);

        Assert.IsFalse(userUI.activeSelf);
        Assert.IsFalse(navUI.activeSelf);
    }

    [Test]
    public void setARMode_enables_UI_when_false()
    {
        xrToggle.setARMode(false);

        Assert.IsTrue(userUI.activeSelf);
        Assert.IsTrue(navUI.activeSelf);
    }
}
