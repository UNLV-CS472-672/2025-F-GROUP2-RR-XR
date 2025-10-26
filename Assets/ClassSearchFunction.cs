using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ClassSearchFunction : MonoBehaviour
{
    [Serializable]
    public class RoomItem
    {
        // room labels for users
        public string label;
        
        // keys for routing
        public string destinationId;
    }

    [SerializeField] private List<RoomItem> items = new List<RoomItem>();
    [SerializeField] private TMP_InputField searchBox;
    [SerializeField] private TextMeshProUGUI resultLabel;

    // event to use for id, destinationID updated when a valid destination is selected
    [SerializeField] private UnityEvent<string> OnDestinationSelected;

    private void Awake()
    {
        if (items == null || items.Count == 0)
        {
            // list of rooms with corresponding IDs
            items = new List<RoomItem>
            {
                new RoomItem { label = "100 COLLABORATORIUM", destinationId = "100" },
                new RoomItem { label = "130 FLEXATORIUM",    destinationId = "130"    },
                new RoomItem { label = "140 CLASSROOM",      destinationId = "140"      },
                new RoomItem { label = "145 CLASSROOM",      destinationId = "145"      },
                new RoomItem { label = "160 MAKER SPACE",    destinationId = "160"    },
                new RoomItem { label = "150 FLEXIBLE CLASSROOM", destinationId = "150" }
            };
        }
    }

    // called by search button
    // show matches to entered query
    public void OnSearchClicked()
    {
        string query = (searchBox.text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(query))
        {
            Show("No search query was entered...");
            return;
        }

        var matches = SearchRooms(query);
        if (matches.Count == 0)
        {
            Show($"No match for: {query}...");
            return;
        }

        Show(PrintRooms(matches));
    }

    // called by go button
    // attempts an exact selection based on input
    public void OnConfirmSelection()
    {
        string label = (searchBox.text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(label))
        {
            Show("No search query was entered...");
            return;
        }
        SelectByLabel(label);
    }

    // select a room by its exact label
    public void SelectByLabel(string label)
    {
        // get the index of the given label
        int labelIndex = FindIndex(label);
        
        // validate index
        if (labelIndex >= 0)
        {
            // get the room from the index
            SelectByIndex(labelIndex);
        }else{
            Show($"No match for: {label}...");
        }
    }

    // select a room by its index
    public void SelectByIndex(int index)
    {
        // check if out of bounds
        if (index < 0 || index >= items.Count)
        {
            return;
        }
        
        // get the room value using the index
        var chosen = items[index];

        // get the destination ID if it exists otherwise use the label
        var id = string.IsNullOrEmpty(chosen.destinationId) ? chosen.label : chosen.destinationId;

        // return key for routing
        OnDestinationSelected?.Invoke(id);

        // print user messages
        Show($"Match found: {chosen.label}");
    }

    // finds room labels that contains the search query
    private List<int> SearchRooms(string query)
    {
        var matches = new List<int>();
        if (items == null || items.Count == 0)
        {
            return matches;
        }
        
        // for every room
        for (int i = 0; i < items.Count; i++)
        {
            // get label of current room
            string s = items[i].label ?? string.Empty;

            // compare it to the query
            if (s.Contains(query))
            {   
                // add to list of matches if match
                matches.Add(i);
            }
        }
        // return list of matches to print
        return matches;
    }

    // find an index using the label
    private int FindIndex(string label)
    {
        if (items == null || items.Count == 0)
        {
            return -1;
        }

        // loop through list of rooms
        for (int i = 0; i < items.Count; i++)
        {
            string s = items[i].label ?? string.Empty;
            
            // compare each label to search queary 
            if (string.Equals(s, label, StringComparison.Ordinal))
                return i;
        }
        return -1;
    }

    // print a list of matches to the search query
    private string PrintRooms(List<int> indices)
    {
        if (indices.Count == 0)
        {
            return "No matches...";
        }

        var lines = new List<string>();

        // for each index add its label to the list to print
        for (int i = 0; i < indices.Count; i++)
        {
            lines.Add($"{i + 1}. {items[indices[i]].label}");
        }
        return string.Join("\n", lines);
    }

    // print user messages
    private void Show(string message)
    {
        if (resultLabel != null)
        {
            resultLabel.text = message;
        }
    }
}