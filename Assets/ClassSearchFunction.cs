using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the classroom/room search functionality for the AR navigation system.
/// Provides real-time search with partial matching, dynamic result display, and room selection.
/// </summary>
/// <remarks>
/// This component handles:
/// - Room database management
/// - Live search query processing with partial string matching
/// - Dynamic UI button generation for search results
/// - Room selection and navigation destination updates
/// </remarks>
public class ClassSearchFunction : MonoBehaviour
{
    /// <summary>
    /// Represents a single room/classroom entry with its metadata.
    /// </summary>
    [Serializable]
    public class RoomItem
    {
        /// <summary>
        /// Display name of the room (e.g., "100 COLLABORATORIUM", "145 CLASSROOM")
        /// </summary>
        public string roomName;

        /// <summary>
        /// Building identifier for routing purposes (e.g., "AEB", "TBE-A", "TBE-B")
        /// </summary>
        public string buildingName;
    }

    [Header("Room Database")]
    [SerializeField]
    [Tooltip("List of all available rooms/classrooms in the system")]
    private List<RoomItem> items = new List<RoomItem>();

    [Header("UI References")]
    [SerializeField]
    [Tooltip("Input field for user search queries")]
    private TMP_InputField searchBox;

    [SerializeField]
    [Tooltip("Text label for displaying search status messages")]
    private TextMeshProUGUI resultLabel;

    [SerializeField]
    [Tooltip("Container that holds dynamically generated result buttons")]
    private RectTransform resultsContainer;

    [SerializeField]
    [Tooltip("Prefab template for individual result buttons")]
    private Button resultButtonPrefab;

    [Header("Events")]
    [SerializeField]
    [Tooltip("Event triggered when user selects a destination. Passes the room name as parameter.")]
    private UnityEvent<string> OnDestinationSelected;

    /// <summary>
    /// Initializes the room database and sets up search functionality.
    /// Called before the first frame update.
    /// </summary>
    private void Awake()
    {
        // Initialize room database if empty
        if (items == null || items.Count == 0)
        {
            // List of rooms with corresponding building IDs
            // TODO: Move this to a JSON configuration file for easier updates
            items = new List<RoomItem>
            {
                // AEB (Applied Engineering Building) rooms
                new RoomItem { roomName = "100 COLLABORATORIUM", buildingName = "AEB" },
                new RoomItem { roomName = "130 FLEXATORIUM", buildingName = "AEB" },
                new RoomItem { roomName = "140 CLASSROOM", buildingName = "AEB" },
                new RoomItem { roomName = "145 CLASSROOM", buildingName = "AEB" },
                new RoomItem { roomName = "160 MAKER SPACE", buildingName = "AEB" },
                new RoomItem { roomName = "150 FLEXIBLE CLASSROOM", buildingName = "AEB" },

                // TBE-A rooms (placeholder data)
                new RoomItem { roomName = "test", buildingName = "TBE-A" },

                // TBE-B rooms (placeholder data)
                new RoomItem { roomName = "test", buildingName = "TBE-B" }
            };
        }

        // Set up live search - results update on each keystroke
        if (searchBox != null)
        {
            searchBox.onValueChanged.AddListener(UpdateResultsList);

            // Perform initial search with current text (if any)
            UpdateResultsList((searchBox.text ?? string.Empty).Trim());
        }

        // Ensure results area starts clean
        if (resultsContainer != null)
        {
            ClearResults();
        }
    }

    /// <summary>
    /// Updates the search results list based on the user's query.
    /// Called automatically whenever the search box text changes.
    /// </summary>
    /// <param name="raw">The raw input text from the search box</param>
    private void UpdateResultsList(string raw)
    {
        // Sanitize input
        string query = (raw ?? string.Empty).Trim();

        // Clear previous search results
        ClearResults();

        // Empty query - clear results and status message
        if (string.IsNullOrEmpty(query))
        {
            Show("");
            return;
        }

        // Search for matching rooms
        var matches = SearchRooms(query);

        // No matches found
        if (matches.Count == 0)
        {
            Show($"No match found for: {query}");
            return;
        }

        // Create interactive buttons for each match
        CreateButtons(matches);
    }

    /// <summary>
    /// Selects a room by its exact name (case-insensitive).
    /// </summary>
    /// <param name="label">The room name to search for</param>
    public void SelectByName(string label)
    {
        // Find the index of the room with this name
        int labelIndex = FindIndex(label);

        // Validate and select
        if (labelIndex >= 0)
        {
            SelectByIndex(labelIndex);
        }
        else
        {
            Show($"No match found for: {label}");
        }
    }

    /// <summary>
    /// Selects a room by its index in the items list.
    /// Triggers the OnDestinationSelected event and updates the UI.
    /// </summary>
    /// <param name="index">Zero-based index of the room to select</param>
    public void SelectByIndex(int index)
    {
        // Bounds checking
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning($"SelectByIndex: Index {index} out of range [0, {items.Count - 1}]");
            return;
        }

        // Get the selected room
        var chosen = items[index];

        // Notify listeners (typically the navigation system)
        OnDestinationSelected?.Invoke(chosen.roomName);

        // Update user feedback
        Show($"Room selected: {chosen.roomName}");
    }

    /// <summary>
    /// Searches for rooms that contain the query string (case-insensitive partial matching).
    /// </summary>
    /// <param name="query">The search query string</param>
    /// <returns>List of indices for rooms that match the query</returns>
    private List<int> SearchRooms(string query)
    {
        var matches = new List<int>();

        // Error checking
        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("SearchRooms: Room database is empty");
            return matches;
        }

        // Iterate through all rooms
        for (int i = 0; i < items.Count; i++)
        {
            string roomName = items[i].roomName ?? string.Empty;

            // Case-insensitive partial match using IndexOf
            // This allows queries like "flex" to match "FLEXATORIUM"
            if (roomName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                matches.Add(i);
            }
        }

        return matches;
    }

    /// <summary>
    /// Finds the index of a room by exact name match (case-insensitive).
    /// </summary>
    /// <param name="label">The exact room name to find</param>
    /// <returns>The index of the room, or -1 if not found</returns>
    private int FindIndex(string label)
    {
        // Error check
        if (items == null || items.Count == 0)
        {
            return -1;
        }

        // Search for exact match
        for (int i = 0; i < items.Count; i++)
        {
            string roomName = items[i].roomName ?? string.Empty;

            // Case-insensitive exact comparison
            if (string.Equals(roomName, label, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Removes all dynamically created result buttons from the results container.
    /// </summary>
    private void ClearResults()
    {
        if (resultsContainer == null) return;

        // Destroy all child objects (result buttons) in reverse order
        // Reverse iteration prevents index shifting issues during destruction
        for (int i = resultsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(resultsContainer.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Creates interactive UI buttons for each search result.
    /// Each button displays the room name and building, and triggers SelectByIndex when clicked.
    /// </summary>
    /// <param name="indices">List of room indices to create buttons for</param>
    private void CreateButtons(List<int> indices)
    {
        if (resultButtonPrefab == null || resultsContainer == null)
        {
            Debug.LogError("CreateButtons: Missing button prefab or results container reference");
            return;
        }

        // Create a button for each matching room
        for (int i = 0; i < indices.Count; i++)
        {
            int actualIndex = indices[i];

            // Instantiate button from prefab
            var btn = Instantiate(resultButtonPrefab, resultsContainer);

            // Find and populate text fields within the button
            // The button prefab should contain TextMeshProUGUI children named "RoomName" and "BuildingName"
            var labelTexts = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var text in labelTexts)
            {
                if (text.name.Contains("RoomName", StringComparison.OrdinalIgnoreCase))
                    text.text = items[actualIndex].roomName;
                else if (text.name.Contains("BuildingName", StringComparison.OrdinalIgnoreCase))
                    text.text = items[actualIndex].buildingName;
            }

            // Clear any existing click handlers
            btn.onClick.RemoveAllListeners();

            // Set up click handler to select this room
            // Note: Capture actualIndex in a closure for the lambda
            btn.onClick.AddListener(() => SelectByIndex(actualIndex));
        }
    }

    /// <summary>
    /// Displays a status message to the user in the result label.
    /// Supports rich text formatting.
    /// </summary>
    /// <param name="message">The message to display</param>
    private void Show(string message)
    {
        if (resultLabel != null)
        {
            resultLabel.richText = true;
            resultLabel.text = message;
        }
    }
}
