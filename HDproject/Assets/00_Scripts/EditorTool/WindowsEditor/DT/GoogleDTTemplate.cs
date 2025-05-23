// GoogleDTTemplate.cs

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public static class ScriptableObjectTemplateGenerator
{
    public static void GenerateTemplateClass(string className, List<string> fields, List<string> types, string folder)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine($"\n[CreateAssetMenu(fileName = \"{className}\", menuName = \"Data/{className}\")]\npublic class {className} : ScriptableObject\n{{");


        for (int i = 0; i < fields.Count; i++)
        {
            sb.AppendLine($"    public {types[i]} {fields[i]};");
        }

        sb.AppendLine("}");

        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, className + ".cs"), sb.ToString());
        AssetDatabase.Refresh();
    }
}

public static class TypeInference
{
    public static List<string> InferFieldTypes(List<List<string>> rows)
    {
        var types = new List<string>();
        if (rows.Count == 0) return types;

        int colCount = rows[0].Count;
        for (int i = 0; i < colCount; i++)
        {
            bool isInt = true, isFloat = true, isBool = true;

            foreach (var row in rows)
            {
                if (i >= row.Count) continue;
                string val = row[i].Trim();
                if (!int.TryParse(val, out _)) isInt = false;
                if (!float.TryParse(val, out _)) isFloat = false;
                if (val != "true" && val != "false") isBool = false;
            }

            if (isInt) types.Add("int");
            else if (isFloat) types.Add("float");
            else if (isBool) types.Add("bool");
            else types.Add("string");
        }
        return types;
    }
}


public class GoogleDTTemplate : MonoBehaviour
{
    public string spreadsheetId;
    public string sheetName;

    void Start()
    {
        string csvUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
        List<List<string>> data = GoogleSheetLoader.LoadCSVFromUrl(csvUrl);
        if (data == null || data.Count == 0) return;

        List<string> headers = data[0];
        List<List<string>> rows = data.GetRange(1, data.Count - 1);
        List<string> types = TypeInference.InferFieldTypes(rows);

        ScriptableObjectTemplateGenerator.GenerateTemplateClass(sheetName, headers, types, "Assets/Scripts/Generated");
        Debug.Log($"Template for {sheetName} generated.");

        // 컴파일 후 타입 생성된 뒤 실행 필요
        Type type = Type.GetType(sheetName);
        if (type == null)
        {
            Debug.LogError($"타입 {sheetName} 을(를) 찾을 수 없습니다. 스크립트 컴파일 후 다시 실행해주세요.");
            return;
        }

        string saveFolder = $"Assets/Resources/Generated/{sheetName}";
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        for (int i = 0; i < rows.Count; i++)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            var fields = type.GetFields();
            var row = rows[i];

            for (int j = 0; j < headers.Count && j < row.Count; j++)
            {
                var field = type.GetField(headers[j]);
                if (field == null) continue;

                string val = row[j];
                object parsed = types[j] switch
                {
                    "int" => int.TryParse(val, out int iv) ? iv : 0,
                    "float" => float.TryParse(val, out float fv) ? fv : 0f,
                    "bool" => val == "true" || val == "1",
                    _ => val
                };
                field.SetValue(asset, parsed);
            }

            string assetPath = Path.Combine(saveFolder, row[0] + ".asset");
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"ScriptableObjects for {sheetName} generated.");
    }
}