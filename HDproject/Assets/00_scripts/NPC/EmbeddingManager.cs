using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class EmbeddingResponse
{
    public string model;
    public EmbeddingData[] data;
    public Usage usage;
}

[System.Serializable]
public class EmbeddingData
{
    public string @object;
    public float[] embedding;
    public int index;
}

[System.Serializable]
public class Usage
{
    public int prompt_tokens;
    public int total_tokens;
}

public class EmbeddingManager : MonoBehaviour
{
    private const string embeddingUrl = "http://127.0.0.1:1234/v1/embeddings";

    public IEnumerator RequestEmbeddings(string concept, string prompt, System.Action<float[][]> onComplete = null)
    {
        string json = @"
        {
            ""model"": ""text-embedding-nomic-embed-text-v1.5"",
            ""input"": [
                """ + EscapeForJson(concept) + @""",
                """ + EscapeForJson(prompt) + @"""
            ]
        }";

        UnityWebRequest request = new UnityWebRequest(embeddingUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<EmbeddingResponse>(request.downloadHandler.text);
            float[][] vectors = new float[resp.data.Length][];
            for (int i = 0; i < resp.data.Length; i++)
                vectors[i] = resp.data[i].embedding;
            onComplete?.Invoke(vectors);
        }
        else
        {
            Debug.LogError($"Embedding request failed: {request.error}");
            onComplete?.Invoke(null);
        }
    }

    private string EscapeForJson(string s)
    {
        return string.IsNullOrEmpty(s)
            ? ""
            : s.Replace("\\", "\\\\")
               .Replace("\"", "\\\"")
               .Replace("\n", "\\n")
               .Replace("\r", "\\r");
    }
}
