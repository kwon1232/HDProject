using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LocalLLMChatManager : MonoBehaviour
{
    public string apiUrl = "http://127.0.0.1:1234";

    public IEnumerator GetChatResponse(string concept, string prompt, System.Action<string> onResult)
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
        // UploadHandlerRaw is General constructor. Contents of the input argument are copied into a native buffer.
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            string reply = ParseResponse(jsonResult);
            onResult.Invoke(reply);
        }
        else
        {
            onResult.Invoke("에러: " + request.error);
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
