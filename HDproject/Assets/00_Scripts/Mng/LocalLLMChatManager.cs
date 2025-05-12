using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LocalLLMChatManager : MonoBehaviour
{
    [Header("Model Server Settings")]
    public string apiUrl = "http://127.0.0.1:1234"; // LM Studio 주소

    [Header("UI 연결")]
    public DialogueUI dialogueUI;                  //  연결 필수
    public Sprite npcPortrait;                     //  초상화 이미지 (선택)

    private string prefabPath = "UI/UIDialogue";

    public DialogueUI CreateUIDialogue()
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            return null;
        }
        GameObject instance = Instantiate(prefab);
        dialogueUI = instance.GetComponent<DialogueUI>();
        return dialogueUI;

    }

    public void Initialize()
    {
        if (dialogueUI == null) this.CreateUIDialogue();

        // 각종 초기화 세팅 여기서 추가
    }

    public IEnumerator GetChatResponse(string concept, string prompt, System.Action<object> value)
    {
        string json = @"
        {
            ""model"" : ""local-model"",
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            string reply = ParseResponse(jsonResult);

            // 대화 UI에 출력
            if (dialogueUI != null)
                dialogueUI.ShowText(reply, npcPortrait); // 초상화 함께 출력
        }
        else
        {
            if (dialogueUI != null)
                dialogueUI.ShowText("에러: " + request.error);
        }
    }

    private string ParseResponse(string json)
    {
        var result = JsonUtility.FromJson<ChatResponseWrapper>(json);
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
