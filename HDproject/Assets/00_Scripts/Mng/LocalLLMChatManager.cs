using System;
using System.Collections;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class LocalLLMChatManager : MonoBehaviour
{
    [Header("Model Server Settings")]
    public const string apiUrl = "http://192.168.55.182:1234/v1/chat/completions";

    [SerializeField] private NPCInteraction npcInteraction;

    public IEnumerator SendPrompt(string concept, string prompt, Action<object> value)
    {
        string json = @"
        {
            ""model"" : ""deepseek-r1-distill-qwen-7b"",
            ""messages"": [
                { ""role"": ""system"",  ""content"": """ + EscapeForJson(concept) + @""" },
                { ""role"": ""user"",    ""content"": """ + EscapeForJson(prompt) + @""" }
            ]
        }";

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Raw JSON Response: " + jsonResult);

            // ParseResponse가 string[]을 반환하도록 변경
            string[] lines = ParseResponse(jsonResult);

            npcInteraction.GetDialogueUI().PlayDialogue(lines, null);
            value?.Invoke(lines);
        }
        else
        {
            string[] lines = new string[1];
            string str = "에러: " + request.error;
            lines.AddRange(str);
            
            Debug.LogError(request.error);
            npcInteraction.GetDialogueUI().PlayDialogue(lines, null);
            value?.Invoke(null);
        }
    }

    // 반환형을 string[] 으로, 입력 파라미터는 string json 으로 수정
    private string[] ParseResponse(string json)
    {
        var result = JsonUtility.FromJson<ChatResponseWrapper>(json);
        string content = result?.choices?[0]?.message?.content ?? "";

        // <think>…</think> 블록 제거
        int t0 = content.IndexOf("<think>", StringComparison.OrdinalIgnoreCase);
        int t1 = content.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
        if (t0 >= 0 && t1 > t0)
            content = content.Remove(t0, t1 + 8 - t0);

        const string startTag = "start NPC descript";
        const string endTag = "end NPC descript";

        // 최초 startTag 위치, 마지막 endTag 위치 찾기
        int start = content.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
        int end = content.LastIndexOf(endTag, StringComparison.OrdinalIgnoreCase);

        if (start >= 0 && end > start)
        {
            int bodyStart = start + startTag.Length;
            content = content.Substring(bodyStart, end - bodyStart);
        }

        // 앞뒤 공백·줄바꿈 제거
        content = content.Trim();

        // 줄바꿈 단위로 분리해 배열로 반환
        var rawLines = content
            .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rawLines.Length; i++)
            rawLines[i] = rawLines[i].Trim();
        return rawLines;
    }

    private string EscapeForJson(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
    }

    [Serializable] private class ChatResponseWrapper { public Choice[] choices; }
    [Serializable] private class Choice { public Message message; }
    [Serializable] private class Message { public string content; }
}
