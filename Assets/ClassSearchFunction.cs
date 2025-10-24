using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClassSearchFunction : MonoBehaviour
{
    [SerializeField] private List<string> items = new List<string>();
    [SerializeField] private TMP_InputField searchBox;
    [SerializeField] private TextMeshProUGUI resultLabel;

    private void Awake()
    {
        if (items == null || items.Count == 0)
        {
            items = new List<string>
            {
                "100 COLLABORATORIUM",
                "130 FLEXATORIUM",
                "140 CLASSROOM",
                "145 CLASSROOM",
                "160 MAKER SPACE",
                "150 FLEXIBLE CLASSROOM"
            };
        }
    }

    public void OnSearchClicked()
    {
        string query = (searchBox.text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(query))
        {
            Show("no search queary entered");
            return;
        }

        int foundIndex = findIndex(query); // used later in program for destination.
        if (foundIndex >= 0)
        {
            Show($"match found: {items[foundIndex]}");
        }
        else
        {
            Show($"no match: {query}");
        }
    }

    private int findIndex(string query)
    {
        if (items == null || items.Count == 0) return -1;

        for (int i = 0; i < items.Count; i++)
        {
            string s = items[i] ?? string.Empty;
            if (string.Equals(s, query, StringComparison.Ordinal))
                return i;
        }
        return -1;
    }

    private void Show(string message)
    {
        if (resultLabel != null) resultLabel.text = message;
        Debug.Log(message);
    }
}