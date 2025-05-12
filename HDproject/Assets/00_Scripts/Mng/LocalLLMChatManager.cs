using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LocalLLMChatManager : MonoBehaviour
{
    [Header("Model Server Settings")]
    public const string apiUrl = "http://192.168.55.182:1234/v1/chat/completions";    // LM Studio 주소       

    [SerializeField] private NPCInteraction npcInteraction;
    private string data;


    public IEnumerator SendPrompt(string concept, string prompt, System.Action<object> value)
    {
        string json = @"
        {
            ""model"" : ""deepseek-r1-distill-qwen-7b"",
            ""messages"": [
                {
                    ""role"": ""system"",
                    ""content"": """ + concept + @"""
                },
                {
                    ""role"": ""user"",
                    ""content"": """ + prompt + @"""
                }
            ]
        }";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Raw JSON Response: " + jsonResult);
            string reply = ParseResponse(jsonResult);

            npcInteraction.GetDialogueUI().ShowText(reply, null); // 초상화 함께 출력
        }
        else
        {
            Debug.Log(request.error);
                npcInteraction.GetDialogueUI().ShowText("에러: " + request.error);
        }
    }

    private string ParseResponse(string json)
    {
        var result = JsonUtility.FromJson<ChatResponseWrapper>(json);

        if (result?.choices == null || result.choices.Length == 0)
        {
            Debug.LogWarning("LM 응답이 비어있거나 올바르지 않습니다: " + json);
            return "LM 응답이 없습니다.";
        }

        return result.choices[0].message.content.Trim();
    }

    [System.Serializable]
    private class ChatResponseWrapper
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
}
