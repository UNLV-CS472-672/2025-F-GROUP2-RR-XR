using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ClassSearchFunction : MonoBehaviour
{
    [Serializable]
    public class RoomItem
    {
        // room names for users
        public string roomName;

        // keys for routing
        public string buildingName;
    }

    [SerializeField] private List<RoomItem> items = new List<RoomItem>();
    [SerializeField] private TMP_InputField searchBox;
    [SerializeField] private TextMeshProUGUI resultLabel;
    [SerializeField] private RectTransform resultsContainer;
    [SerializeField] private Button resultButtonPrefab;

    // event to use for id, destinationID updated when a valid destination is selected
    [SerializeField] private UnityEvent<string> OnDestinationSelected;

    private void Awake()
    {
        if (items == null || items.Count == 0)
        {
            // list of rooms with corresponding building names
            items = new List<RoomItem>
            {
                // AEB rooms
                new RoomItem { roomName = "100 COLLABORATORIUM", buildingName = "AEB" },
                new RoomItem { roomName = "130 FLEXATORIUM", buildingName = "AEB" },
                new RoomItem { roomName = "140 CLASSROOM", buildingName = "AEB" },
                new RoomItem { roomName = "145 CLASSROOM", buildingName = "AEB" },
                new RoomItem { roomName = "160 MAKER SPACE", buildingName = "AEB" },
                new RoomItem { roomName = "150 FLEXIBLE CLASSROOM", buildingName = "AEB" },
                // TBE-A rooms
                new RoomItem { roomName = "101 GREAT HALL", buildingName = "TBE-A" },
                new RoomItem { roomName = "107 CLASSROOM", buildingName = "TBE-A" },
                new RoomItem { roomName = "120 MEETING ROOM", buildingName = "TBE-A" },
                new RoomItem { roomName = "305 DATA CENTER", buildingName = "TBE-A" },
                new RoomItem { roomName = "307 CONFERENCE ROOM", buildingName = "TBE-A" },
                new RoomItem { roomName = "311 COMPUTER LAB", buildingName = "TBE-A" },
                // TBE-B rooms
                new RoomItem { roomName = "170 CLASSROOM", buildingName = "TBE-B" },
                new RoomItem { roomName = "172 CLASSROOM", buildingName = "TBE-B" },
                new RoomItem { roomName = "174 CLASSROOM", buildingName = "TBE-B" },
                new RoomItem { roomName = "176 CLASSROOM", buildingName = "TBE-B" },
                new RoomItem { roomName = "178 CLASSROOM", buildingName = "TBE-B" }
            };
        }

        // show matches to entered query
        // results update live on each keystroke
        if (searchBox != null)
        {
            searchBox.onValueChanged.AddListener(UpdateResultsList);

            // update the results list with the current text in the searchbox
            UpdateResultsList((searchBox.text ?? string.Empty).Trim());
        }

        // ensure results area is clean on start
        if (resultsContainer != null)
        {
            ClearResults();
        }
    }

    private void UpdateResultsList(string raw)
    {
        string query = (raw ?? string.Empty).Trim();

        // clear previous buttons
        ClearResults();

        if (string.IsNullOrEmpty(query))
        {
            Show("");
            return;
        }

        // call function to search rooms 
        var matches = SearchRooms(query);
        if (matches.Count == 0)
        {
            Show($"No match found for: {query}");
            return;
        }

        // create buttons for matched searches
        CreateButtons(matches);
    }

    // select a room by its name
    public void SelectByName(string label)
    {
        // get the index of the given room name
        int labelIndex = FindIndex(label);

        // validate index
        if (labelIndex >= 0)
        {
            // get the room from the index
            SelectByIndex(labelIndex);
        }
        else
        {
            Show($"No match found for: {label}");
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

        // return key for routing
        OnDestinationSelected?.Invoke(chosen.roomName);

        // print user messages
        Show($"Room selected: {chosen.roomName}");
    }

    // finds room labels that contains the search query
    private List<int> SearchRooms(string query)
    {
        // list to store matches
        var matches = new List<int>();
  
        // error checking
        if (items == null || items.Count == 0)
        {
            return matches;
        }

        // for every room
        for (int i = 0; i < items.Count; i++)
        {
            // get label of current room and its building name
            string room = items[i].roomName ?? string.Empty;
            string building = items[i].buildingName ?? string.Empty;

            // compare both fields to the query (not case sensitive)
            if (room.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                building.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
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
        // error check
        if (items == null || items.Count == 0)
        {
            return -1;
        }

        // for each room
        for (int i = 0; i < items.Count; i++)
        {

            // get the room name
            string s = items[i].roomName ?? string.Empty;

            // compare each name to the search query (not case sensitive)
            if (string.Equals(s, label, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    // delete existing results under the results container
    private void ClearResults()
    {
        // error check
        if (resultsContainer == null) return;

        // for each result delete it
        for (int i = resultsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(resultsContainer.GetChild(i).gameObject);
        }
    }

    // create a button for each match
    private void CreateButtons(List<int> indices)
    {
        // for each item in the list
        for (int i = 0; i < indices.Count; i++)
        {
            // get the actual index of the room from the list
            int actualIndex = indices[i];

            // make a new button using the prefab button template in the results list container
            var btn = Instantiate(resultButtonPrefab, resultsContainer);

            // find and set the labels of the button to the actual room and building name
            var labelTexts = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var text in labelTexts)
            {
                if (text.name.Contains("RoomName", StringComparison.OrdinalIgnoreCase))
                    text.text = items[actualIndex].roomName;
                else if (text.name.Contains("BuildingName", StringComparison.OrdinalIgnoreCase))
                    text.text = items[actualIndex].buildingName;
            }

            // clear existing listeners
            btn.onClick.RemoveAllListeners();
            
            // set to call select by index function on button click
            btn.onClick.AddListener(() => SelectByIndex(actualIndex));
        }
    }

    // print user messages
    private void Show(string message)
    {
        if (resultLabel != null)
        {
            // parse result tags as rich text
            resultLabel.richText = true;
            resultLabel.text = message;
        }
    }
}
