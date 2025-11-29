using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Assumes you are using TextMeshPro for UI
using UnityEngine.UI;

public class RebelGuideAI : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private TMP_InputField apiKeyInputField; // Drag your new API Key input box here
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent";

    [Header("UI Components")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplayOutput;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject loadingIndicator; // Optional: A spinner or text

    // Preserved your specific system prompt
    private string systemPrompt = "You are Rebel Guide, the enthusiastic AI assistant for the Rebel Reality AR navigation app at UNLV. Your goal is to help students navigate campus (especially Advanced Engineering Building (AEB) or Thomas Beam Engineering Complex (TBE)). Do not provide guidance for routes you do not know how to navigate. Keep answers concise (under 45 words) and friendly. Do not use emojis.";

    void Start()
    {
        // Set up button listener
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendClicked);
            
        if (loadingIndicator != null) 
            loadingIndicator.SetActive(false);

        // --- NEW: Load the API Key from local storage ---
        if (apiKeyInputField != null)
        {
            // 1. Check if we saved it before
            string savedKey = PlayerPrefs.GetString("GeminiAPIKey", "");
            apiKeyInputField.text = savedKey;

            // 2. Listen for changes so we save automatically when you type
            apiKeyInputField.onValueChanged.AddListener(SaveApiKey);
        }
    }
    // Helper function to save key to computer registry
    public void SaveApiKey(string newKey)
    {
        PlayerPrefs.SetString("GeminiAPIKey", newKey);
        PlayerPrefs.Save();
    }

    public void OnSendClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            StartCoroutine(SendRequestToGemini(inputField.text));
        }
    }

    private IEnumerator SendRequestToGemini(string userMessage)
    {
        // 1. Get Key from the UI Field (NOT hardcoded)
        string currentApiKey = "";
        if (apiKeyInputField != null)
        {
            currentApiKey = apiKeyInputField.text.Trim();
        }

        if (string.IsNullOrEmpty(currentApiKey))
        {
            chatDisplayOutput.text += $"\n<color=red>Error: API Key is missing! Please input it in settings.</color>";
            yield break; // Stop execution
        }

        // 2. Show user message in UI immediately
        chatDisplayOutput.text += $"\n\n<color=#007AFF><b>You:</b></color> {userMessage}";
        inputField.text = ""; // Clear input
        
        // --- AUTO-SCROLL (User Message) ---
        Canvas.ForceUpdateCanvases();
        var scrollRect = chatDisplayOutput.GetComponentInParent<ScrollRect>();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f; 
        // ----------------------------------

        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        // 3. Construct the JSON payload
        GeminiRequest request = new GeminiRequest();
        request.contents = new Content[] { 
            new Content { parts = new Part[] { new Part { text = userMessage } } } 
        };
        request.systemInstruction = new SystemInstruction {
            parts = new Part[] { new Part { text = systemPrompt } }
        };

        string jsonPayload = JsonUtility.ToJson(request);

        // 4. Create the Web Request
        string url = $"{apiUrl}?key={currentApiKey}";
        var webRequest = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // 5. Send and Wait
        yield return webRequest.SendWebRequest();

        if (loadingIndicator != null) loadingIndicator.SetActive(false);

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gemini API Error: " + webRequest.error);
            chatDisplayOutput.text += $"\n<color=red>Error: {webRequest.error}</color>";
        }
        else
        {
            // 6. Parse Response
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(webRequest.downloadHandler.text);
            
            if (response != null && response.candidates != null && response.candidates.Length > 0)
            {
                string aiText = response.candidates[0].content.parts[0].text;
                // Add AI response to UI
                chatDisplayOutput.text += $"\n<color=#B10202><b>Rebel Guide:</b></color> {aiText}";

                // --- AUTO-SCROLL (AI Response) ---
                Canvas.ForceUpdateCanvases();
                if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
                // ---------------------------------
            }
        }
    }
}

// --- JSON Data Structures ---

[System.Serializable]
public class GeminiRequest
{
    public Content[] contents;
    public SystemInstruction systemInstruction;
}

[System.Serializable]
public class SystemInstruction
{
    public Part[] parts;
}

[System.Serializable]
public class Content
{
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}

[System.Serializable]
public class GeminiResponse
{
    public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}