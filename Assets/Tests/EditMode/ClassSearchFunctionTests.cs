using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using Immersal.Samples.Navigation;

public class ClassSearchFunctionTests
{
    private GameObject root;
    private ClassSearchFunction searchFunc;

    private TMP_InputField inputField;
    private TextMeshProUGUI resultLabel;
    private RectTransform resultsContainer;
    private NavigationTargetListButton buttonPrefab;

    [SetUp]
    public void Setup()
    {
        root = new GameObject("Root");
        searchFunc = root.AddComponent<ClassSearchFunction>();

        // Mock input field
        var inputObj = new GameObject("InputField");
        inputField = inputObj.AddComponent<TMP_InputField>();
        searchFunc.GetType().GetField("searchBox", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, inputField);

        // Mock result label
        var labelObj = new GameObject("ResultLabel");
        resultLabel = labelObj.AddComponent<TextMeshProUGUI>();
        searchFunc.GetType().GetField("resultLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, resultLabel);

        // Mock results container
        var containerObj = new GameObject("ResultsContainer");
        resultsContainer = containerObj.AddComponent<RectTransform>();
        searchFunc.GetType().GetField("resultsContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, resultsContainer);

        // Mock button prefab
        var buttonObj = new GameObject("ButtonPrefab");
        buttonPrefab = buttonObj.AddComponent<NavigationTargetListButton>();
        searchFunc.GetType().GetField("resultButtonPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, buttonPrefab);

        // Mock XRToggle
        var xrObj = new GameObject("XRToggle");
        searchFunc.GetType().GetField("arToggle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, xrObj.AddComponent<XRToggle>());
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(inputField.gameObject);
        Object.DestroyImmediate(resultLabel.gameObject);
        Object.DestroyImmediate(resultsContainer.gameObject);
        Object.DestroyImmediate(buttonPrefab.gameObject);
    }

    // ensures empty query clears results
    [Test]
    public void UpdateResultsList_empty_query()
    {
        inputField.text = "";

        searchFunc.GetType().GetMethod("UpdateResultsList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { inputField.text });

        Assert.AreEqual(0, resultsContainer.childCount);
        Assert.AreEqual("", resultLabel.text);
    }

    // ensures no match found message is displayed
    [Test]
    public void UpdateResultsList_no_match_found()
    {
        inputField.text = "nonexistent";

        searchFunc.GetType().GetMethod("UpdateResultsList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { inputField.text });

        Assert.AreEqual(0, resultsContainer.childCount);
        StringAssert.Contains("No match found", resultLabel.text);
    }

    // ensures correct functionality on valid name
    [Test]
    public void SelectByName_valid_name()
    {
        bool invoked = false;
        searchFunc.GetType().GetField("OnDestinationSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, new UnityEngine.Events.UnityEvent<string>());
        var evt = (UnityEngine.Events.UnityEvent<string>)searchFunc.GetType().GetField("OnDestinationSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(searchFunc);
        evt.AddListener(_ => invoked = true);

        string roomName = "100 COLLABORATORIUM";

        searchFunc.GetType().GetMethod("SelectByName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { roomName });

        Assert.IsTrue(invoked);
        StringAssert.Contains("Room selected", resultLabel.text);
    }

    // ensure label is updated on invalid name
    [Test]
    public void SelectByName_invalid_name()
    {
        string roomName = "NOT A ROOM";

        searchFunc.GetType().GetMethod("SelectByName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { roomName });

        StringAssert.Contains("No match found", resultLabel.text);
    }

    // ensures correct matches
    [Test]
    public void SearchRooms_matches_correctly()
    {
        var matches = (List<int>)searchFunc.GetType().GetMethod("SearchRooms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { "100" });

        Assert.AreEqual(1, matches.Count);
        Assert.AreEqual(0, matches[0]);
    }

    // ensures correct functionality valid index
    [Test]
    public void SelectByIndex_valid_index()
    {
        bool invoked = false;
        var evt = new UnityEngine.Events.UnityEvent<string>();
        evt.AddListener(_ => invoked = true);
        searchFunc.GetType().GetField("OnDestinationSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, evt);

        searchFunc.GetType().GetMethod("SelectByIndex", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { 0 });

        Assert.IsTrue(invoked);
        StringAssert.Contains("Room selected", resultLabel.text);
    }

    // ensures invalid index does nothing
    [Test]
    public void SelectByIndex_invalid_index_does_nothing()
    {
        var evt = new UnityEngine.Events.UnityEvent<string>();
        searchFunc.GetType().GetField("OnDestinationSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(searchFunc, evt);

        searchFunc.GetType().GetMethod("SelectByIndex", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Invoke(searchFunc, new object[] { -1 });

        Assert.AreEqual("", resultLabel.text);
    }
}

