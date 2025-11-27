using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Assumes you are using TextMeshPro for UI
using UnityEngine.UI;

public class RebelGuideAI : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent";

    [Header("UI Components")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplayOutput;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject loadingIndicator; // Optional: A spinner or text

    // System prompt to give the AI its personality
    private string systemPrompt = "You are Rebel Guide, the enthusiastic AI assistant for the Rebel Reality AR navigation app at UNLV. Your goal is to help students navigate campus (especially Advanced Engineering Building (AEB) or Thomas Beam Engineering Complex (TBE)). Do not provide guidance for routes you do not know how to navigate. Keep answers concise (under 45 words) and friendly. Do not use emojis.";

    void Start()
    {
        // Set up button listener
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendClicked);
            
        if (loadingIndicator != null) 
            loadingIndicator.SetActive(false);
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
        // 1. Show user message in UI immediately
        chatDisplayOutput.text += $"\n\n<color=#007AFF><b>You:</b></color> {userMessage}";
        inputField.text = ""; // Clear input
        
        // --- AUTO-SCROLL (User Message) ---
        // Force the canvas to update the text size immediately, then scroll to bottom
        Canvas.ForceUpdateCanvases();
        var scrollRect = chatDisplayOutput.GetComponentInParent<ScrollRect>();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = -10f; 
        // ----------------------------------

        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        // 2. Construct the JSON payload using wrapper classes
        GeminiRequest request = new GeminiRequest();
        request.contents = new Content[] { 
            new Content { parts = new Part[] { new Part { text = userMessage } } } 
        };
        request.systemInstruction = new SystemInstruction {
            parts = new Part[] { new Part { text = systemPrompt } }
        };

        string jsonPayload = JsonUtility.ToJson(request);

        // 3. Create the Web Request
        string url = $"{apiUrl}?key={apiKey}";
        var webRequest = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // 4. Send and Wait
        yield return webRequest.SendWebRequest();

        if (loadingIndicator != null) loadingIndicator.SetActive(false);

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gemini API Error: " + webRequest.error);
            chatDisplayOutput.text += $"\n<color=red>Error: Could not connect to AI.</color>";
        }
        else
        {
            // 5. Parse Response
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

// --- JSON Data Structures (Required for Unity JsonUtility) ---

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