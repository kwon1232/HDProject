using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
            System.Type type = System.Type.GetType(className);
            if (type == null)
            {
                Debug.LogError($"{className} 타입을 찾을 수 없습니다. 컴파일 후 다시 시도하세요.");
                return;
            }

            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            for (int j = 0; j < headers.Count; j++)
            {
                if (j >= rows[i].Count) continue;

                var field = type.GetField(Regex.Replace(headers[j], "[^a-zA-Z0-9_]", "_"));
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

            string fileName = rows[i][0];
            AssetDatabase.CreateAsset(asset, Path.Combine(fullPath, $"{fileName}.asset"));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}


public static class CSGenerator
{
    public static void GenerateStaticClass(string className, string elementClassName, List<string> headers, List<List<string>> rows, string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("public static class " + className);
        sb.AppendLine("{");
        sb.AppendLine($"    public static readonly {elementClassName}[] Items = new {elementClassName}[]");
        sb.AppendLine("    {");

        foreach (var row in rows)
        {
            sb.Append("        new " + elementClassName + " { ");
            for (int i = 0; i < headers.Count; i++)
            {
                string field = Regex.Replace(headers[i], "[^a-zA-Z0-9_]", "_");
                string value = row[i].Replace("\"", "");
                sb.Append($"{field} = \"{value}\"");
                if (i < headers.Count - 1) sb.Append(", ");
            }
            sb.AppendLine(" },");
        }

        sb.AppendLine("    };\n}");

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
    }
}