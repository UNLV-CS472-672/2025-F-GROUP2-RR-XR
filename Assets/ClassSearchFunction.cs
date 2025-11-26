using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ClassSearchFunction : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    public class RoomItem
    {
        // room names for users
        public string roomName;

        // keys for routing
        public string roomID;
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
                new RoomItem { roomName = "100 COLLABORATORIUM", roomID = "100" },
                new RoomItem { roomName = "130 FLEXATORIUM", roomID = "130" },
                new RoomItem { roomName = "140 CLASSROOM", roomID = "140" },
                new RoomItem { roomName = "145 CLASSROOM", roomID = "145" },
                new RoomItem { roomName = "160 MAKER SPACE", roomID = "160" },
                new RoomItem { roomName = "150 FLEXIBLE CLASSROOM", roomID = "150" }
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
    }

    //createes list of clickable results
    private void UpdateResultsList(string raw)
    {
        string query = (raw ?? string.Empty).Trim();
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

        // display the results as links
        Show(CreateLinks(matches));
    }

    // when a result is clicked in the list, select that room item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (resultLabel == null)
        {
            return;
        }

        // check if a result was clicked based on mouse position on the list
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(resultLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            // gets data about the selected result from its index
            var linkInfo = resultLabel.textInfo.linkInfo[linkIndex];
            
            // get the result ID of the selected result 
            string linkId = linkInfo.GetLinkID();

            // convert id string into an int
            if (int.TryParse(linkId, out int actualIndex))
            {
                // look up room at the index
                SelectByIndex(actualIndex);
            }
        }
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

        // get the destination ID if it exists otherwise use the label
        var id = string.IsNullOrEmpty(chosen.roomID) ? chosen.roomName : chosen.roomID;

        // return key for routing
        OnDestinationSelected?.Invoke(id);

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
            // get label of current room
            string s = items[i].roomName ?? string.Empty;

            // compare it to the query to see if it contains it (not case sensitive)
            if (s.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
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

    // build the list of clickable results
    private string CreateLinks(List<int> indices)
    {
        // error check
        if (indices.Count == 0)
        {
            return "No matches found...";
        }

        // list of result lines
        var lines = new List<string>(indices.Count);

        // for every matching index in the results
        for (int i = 0; i < indices.Count; i++)
        {
            // get the actual position of the matching room from the rooms list
            int actualIndex = indices[i];

            // get the actual room name from that index
            string label = items[actualIndex].roomName;

            // create a clickable link to be used as an item in the results list
            lines.Add($"<link={actualIndex}>{label}</link>");
        }
        return string.Join("\n", lines);
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
