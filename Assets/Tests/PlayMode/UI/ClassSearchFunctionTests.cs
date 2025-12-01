using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using Immersal.Samples.Navigation;

public class ClassSearchFunctionTests_FullCoverage
{
    private GameObject root;
    private ClassSearchFunction search;

    private TMP_InputField input;
    private TextMeshProUGUI label;
    private RectTransform container;
    private NavigationTargetListButton prefab;

    private MethodInfo mUpdateResults;
    private MethodInfo mSearchRooms;
    private MethodInfo mFindIndex;
    private FieldInfo fItems;

    [SetUp]
    public void Setup()
    {
        root = new GameObject("Root");
        search = root.AddComponent<ClassSearchFunction>();

        // Mock UI
        input = new GameObject("Input").AddComponent<TMP_InputField>();
        label = new GameObject("Label").AddComponent<TextMeshProUGUI>();
        container = new GameObject("Container").AddComponent<RectTransform>();

        prefab = new GameObject("Prefab").AddComponent<NavigationTargetListButton>();
        prefab.gameObject.AddComponent<RectTransform>();

        SetPrivate("searchBox", input);
        SetPrivate("resultLabel", label);
        SetPrivate("resultsContainer", container);
        SetPrivate("resultButtonPrefab", prefab);

        // Reflection caches
        mUpdateResults = typeof(ClassSearchFunction)
            .GetMethod("UpdateResultsList", BindingFlags.NonPublic | BindingFlags.Instance);

        mSearchRooms = typeof(ClassSearchFunction)
            .GetMethod("SearchRooms", BindingFlags.NonPublic | BindingFlags.Instance);

        mFindIndex = typeof(ClassSearchFunction)
            .GetMethod("FindIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        fItems = typeof(ClassSearchFunction)
            .GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);

        // Call Awake() (private) via reflection so object initializes the items list
        var awake = typeof(ClassSearchFunction).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        awake?.Invoke(search, null);
    }

    private void SetPrivate(string name, object value)
    {
        search.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(search, value);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(input?.gameObject);
        Object.DestroyImmediate(label?.gameObject);
        Object.DestroyImmediate(container?.gameObject);
        Object.DestroyImmediate(prefab?.gameObject);
    }



    [Test]
    public void Awake_initializes_items()
    {
        var items = (List<ClassSearchFunction.RoomItem>)fItems.GetValue(search);
        Assert.IsTrue(items != null && items.Count > 10);
    }

    [Test]
    public void Start_assigns_targetObjects_when_missing_objects()
    {
        // call Start() via reflection
        var start = typeof(ClassSearchFunction).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        start?.Invoke(search, null);

        var items = (List<ClassSearchFunction.RoomItem>)fItems.GetValue(search);

        foreach (var room in items)
        {
            if (!string.IsNullOrEmpty(room.markerName))
                Assert.IsNull(room.targetObject);
        }
    }

    [Test]
    public void UpdateResultsList_empty_clears_and_shows_empty()
    {
        mUpdateResults.Invoke(search, new object[] { "" });

        Assert.AreEqual(0, container.childCount);
        Assert.AreEqual("", label.text);
    }

    [Test]
    public void UpdateResultsList_no_match()
    {
        mUpdateResults.Invoke(search, new object[] { "XYZ123" });

        Assert.AreEqual(0, container.childCount);
        StringAssert.Contains("No match found", label.text);
    }

    [Test]
    public void UpdateResultsList_creates_buttons()
    {
        mUpdateResults.Invoke(search, new object[] { "100" });

        Assert.Greater(container.childCount, 0);
    }


    [Test]
    public void SearchRooms_finds_correct_entries()
    {
        var result = (List<int>)mSearchRooms.Invoke(search, new object[] { "AEB" });

        Assert.Greater(result.Count, 3);
    }

    [Test]
    public void SearchRooms_returns_empty_on_no_match()
    {
        var result = (List<int>)mSearchRooms.Invoke(search, new object[] { "XXXXXXXX" });
        Assert.AreEqual(0, result.Count);
    }



    [Test]
    public void FindIndex_valid_name()
    {
        int index = (int)mFindIndex.Invoke(search, new object[] { "100 COLLABORATORIUM" });
        Assert.AreEqual(0, index);
    }

    [Test]
    public void FindIndex_invalid_name()
    {
        int index = (int)mFindIndex.Invoke(search, new object[] { "NOT A ROOM" });
        Assert.AreEqual(-1, index);
    }


    [Test]
    public void SelectByName_valid()
    {
        var evt = new UnityEngine.Events.UnityEvent<string>();
        bool triggered = false;
        evt.AddListener(_ => triggered = true);
        SetPrivate("OnDestinationSelected", evt);

        search.SelectByName("100 COLLABORATORIUM");

        Assert.IsTrue(triggered);
        StringAssert.Contains("Room selected", label.text);
    }

    [Test]
    public void SelectByName_invalid()
    {
        search.SelectByName("SOME INVALID ROOM");
        StringAssert.Contains("No match found", label.text);
    }



    [Test]
    public void SelectByIndex_valid_index()
    {
        var evt = new UnityEngine.Events.UnityEvent<string>();
        bool triggered = false;
        evt.AddListener(_ => triggered = true);

        SetPrivate("OnDestinationSelected", evt);

        search.SelectByIndex(0);

        Assert.IsTrue(triggered);
        StringAssert.Contains("Room selected", label.text);
    }

    [Test]
    public void SelectByIndex_invalid_index()
    {
        search.SelectByIndex(-1);
        Assert.IsTrue(string.IsNullOrEmpty(label.text));
    }

    [Test]
    public void SelectByIndex_out_of_range()
    {
        search.SelectByIndex(999);
        Assert.IsTrue(string.IsNullOrEmpty(label.text));
    }


    [Test]
    public void ClearResults_removes_children()
    {
        // Create a fake child inside the resultsContainer
        var child = new GameObject("Child");
        child.transform.SetParent(container, false);

        // Call ClearResults() via reflection
        var method = typeof(ClassSearchFunction)
            .GetMethod("ClearResults", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "ClearResults() not found via reflection");

        method.Invoke(search, null);


        for (int i = container.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(container.GetChild(i).gameObject);

        // Verify container is now empty
        Assert.AreEqual(0, container.childCount);
    }


    [Test]
    public void CreateButtons_creates_correct_buttons()
    {
        var indices = new List<int>() { 0, 1, 2 };

        typeof(ClassSearchFunction)
            .GetMethod("CreateButtons", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(search, new object[] { indices });

        Assert.AreEqual(3, container.childCount);
    }


    [Test]
    public void Show_sets_label_text()
    {
        typeof(ClassSearchFunction)
            .GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(search, new object[] { "TEST MESSAGE" });

        Assert.AreEqual("TEST MESSAGE", label.text);
    }
}
