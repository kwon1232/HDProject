using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SOGenerator
{
    public static void Generate(string className, List<string> headers, List<string> types, List<List<string>> rows, string outputPath)
    {
        string fullPath = Path.Combine("Assets/Resources", outputPath);
        if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

        for (int i = 0; i < rows.Count; i++)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(className);
            var fields = asset.GetType().GetFields();
            for (int j = 0; j < headers.Count; j++)
            {
                if (j >= rows[i].Count) continue;

                var field = asset.GetType().GetField(headers[j]);
                if (field == null) continue;

                string raw = rows[i][j];
                object value = types[j] switch
                {
                    "int" => int.TryParse(raw, out int iv) ? iv : 0,
                    "float" => float.TryParse(raw, out float fv) ? fv : 0f,
                    "bool" => raw == "true" || raw == "1",
                    _ => raw
                };
                field.SetValue(asset, value);
            }

            string fileName = rows[i][0]; // 첫 번째 컬럼이 ID라고 가정
            AssetDatabase.CreateAsset(asset, Path.Combine(fullPath, $"{fileName}.asset"));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
