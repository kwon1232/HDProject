using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

public static class DataDirectoryGenerator
{

    public static void Generate(string generatedDir = "Assets/Scripts/Generated/")
    {
        // 1. 모든 cs 파일 탐색
        string[] files = Directory.GetFiles(generatedDir, "*.cs");
        string dataDirPath = Path.Combine(generatedDir, "DataDirectory.cs");
        var entries = new StringBuilder();

        foreach (var file in files)
        {
            // 본인(DataDirectory.cs) 무시
            if (Path.GetFileName(file) == "DataDirectory.cs")
                continue;

            // 클래스명 추출
            string className = Path.GetFileNameWithoutExtension(file);
            // Data 클래스를 따로 분리하고 싶으면 .cs 파일 내 실제 class 정의 파싱해야 하지만
            // 파일명이 ClassName.cs 일 때만 대상으로
            entries.AppendLine($"    public static readonly {className}[] {className}Items = {className}Data.Items;");

        }

        // 2. 코드 생성
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated: All Data Classes Entry Point");
        sb.AppendLine("public static class DataDirectory");
        sb.AppendLine("{");
        sb.Append(entries);
        sb.AppendLine("}");

        File.WriteAllText(dataDirPath, sb.ToString());
        AssetDatabase.Refresh();
    }
}

public static class GenericSheetClassGenerator
{
    public static void Generate(
        string className,
        List<string> headers,
        List<string> types,
        List<List<string>> rows,
        string generatedDir)
    {
        string filePath = Path.Combine(generatedDir, className + ".cs");
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");
        for (int i = 0; i < headers.Count; i++)
            sb.AppendLine($"    public {types[i]} {headers[i]};");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine($"public static class {className}Data");
        sb.AppendLine("{");
        sb.AppendLine($"    public static readonly {className}[] Items = new {className}[]");
        sb.AppendLine("    {");

        foreach (var row in rows)
        {
            sb.AppendLine($"        new {className}");
            sb.AppendLine("        {");
            for (int i = 0; i < headers.Count; i++)
            {
                string value = (i < row.Count) ? row[i].Trim().Trim('"') : "";
                string field = headers[i];
                string type = types[i];

                if (type == "List<string>")
                {
                    // . 으로 split, trim, 빈 값은 제거
                    var items = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append($"            {field} = new List<string> {{ ");
                    for (int k = 0; k < items.Length; k++)
                    {
                        sb.Append($"\"{items[k].Trim()}\"");
                        if (k < items.Length - 1) sb.Append(", ");
                    }
                    sb.AppendLine(" },");
                }
                else if (type == "int")
                {
                    int intVal = 0;
                    int.TryParse(value, out intVal);
                    sb.AppendLine($"            {field} = {intVal},");
                }
                else if (type == "float")
                {
                    float floatVal = 0f;
                    float.TryParse(value, out floatVal);
                    sb.AppendLine($"            {field} = {floatVal}f,");
                }
                else if (type == "bool")
                {
                    string boolVal = (value == "1" || value.ToLower() == "true") ? "true" : "false";
                    sb.AppendLine($"            {field} = {boolVal},");
                }
                else // string
                {
                    sb.AppendLine($"            {field} = \"{value}\",");
                }
            }
            sb.AppendLine("        },");
        }
        sb.AppendLine("    };");
        sb.AppendLine("}");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, sb.ToString());
        AssetDatabase.Refresh();
    }
}




public static class FieldTypeAutoDetect
{
    public static List<string> InferFieldTypes(List<List<string>> rows, List<string> headers)
    {
        var types = new List<string>();
        for (int col = 0; col < headers.Count; col++)
        {
            bool isInt = true, isFloat = true, isBool = true;
            foreach (var row in rows)
            {
                if (col >= row.Count) continue;
                string val = row[col]?.Trim() ?? "";
                if (!int.TryParse(val, out _)) isInt = false;
                if (!float.TryParse(val, out _)) isFloat = false;
                if (val.ToLower() != "true" && val.ToLower() != "false" && val != "0" && val != "1") isBool = false;
            }
            if (isInt)
                types.Add("int");
            else if (isFloat)
                types.Add("float");
            else if (isBool)
                types.Add("bool");
            else
                types.Add("string");
        }
        return types;
    }
}
