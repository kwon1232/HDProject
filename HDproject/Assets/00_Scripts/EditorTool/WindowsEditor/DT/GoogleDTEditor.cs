using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;// 정규 표현식을 사용하기 위한 네임스페이스
using UnityEditor;
using UnityEngine;

// Utility class to extract spreadsheetId and gid from a Google Sheets URL
public static class GoogleSheetURLParser
{
    // Extracts the spreadsheetId from a given Google Sheets URL
    public static string ExtractSpreadsheetId(string url)
    {
        // Match the pattern to find the spreadsheetId in the URL
        // Example: "https://docs.google.com/spreadsheets/d/1A2B3C4D5E6F7G8H9I0J/edit"
        Match spreadsheetIdMatch = Regex.Match(url, @"docs\.google\.com/spreadsheets/d/([a-zA-Z0-9-_]+)");

        // If a match is found, return the captured group (spreadsheetId), otherwise return null
        return spreadsheetIdMatch.Success ? spreadsheetIdMatch.Groups[1].Value : null;
    }

    // Extracts the gid (sheet/tab ID) from a given Google Sheets URL
    public static string ExtractGid(string url)
    {
        // Match the pattern to find the gid in the URL
        // Example: ".../edit#gid=123456789"
        Match gidMatch = Regex.Match(url, @"gid=(\d+)");

        // If a match is found, return the captured group (gid), otherwise return null
        return gidMatch.Success ? gidMatch.Groups[1].Value : null;
    }
}

public class GoogleDTEditor : EditorWindow
{
    private string sheetURL = "";
    private string sheetName = "";
    private string spreadSheetId = "";
    private string gid = "";

    private List<List<string>> tableData;
    private Vector2 scroll;

    [MenuItem("Tools/Google/DT Editor")]
    public static void Open()
    {
        GetWindow<GoogleDTEditor>("Google DT Editor");
    }

    private void OnGUI()
    {
        sheetURL = EditorGUILayout.TextField("Google Sheet URL", sheetURL);

        if (GUILayout.Button("Load Sheet"))
        {
            string id = GoogleSheetURLParser.ExtractSpreadsheetId(sheetURL);
            string gidStr = GoogleSheetURLParser.ExtractGid(sheetURL);

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(gidStr))
            {
                spreadSheetId = id;
                gid = gidStr;
                Debug.Log($"Parsed spreadsheetId: {spreadSheetId}, gid: {gid}");

                string csvUrl = $"https://docs.google.com/spreadsheets/d/{spreadSheetId}/gviz/tq?tqx=out:csv&gid={gid}";
                tableData = GoogleSheetLoader.LoadCSVFromUrl(csvUrl);
                Debug.Log($"{csvUrl} 불러오기 완료, {tableData?.Count ?? 0}행");
            }
        }

        if (tableData != null && tableData.Count > 0)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var row in tableData)
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var cell in row)
                {
                    EditorGUILayout.TextField(cell, GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Generate Static Data Class"))
            {
                List<string> headers = new List<string>();
                for (int i = 0; i < tableData[0].Count; i++)
                {
                    string sanitized = Regex.Replace(tableData[0][i], "[^a-zA-Z0-9_]", "_");
                    headers.Add(sanitized);
                }
                var rows = tableData.GetRange(1, tableData.Count - 1);
                string className = "UIAssetInfo";
                string wrapperClass = className + "Data";
                string path = "Assets/Scripts/Generated/" + wrapperClass + ".cs";

                var sb = new StringBuilder();
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine("\npublic class " + className);
                sb.AppendLine("{");
                foreach (var h in headers)
                {
                    sb.AppendLine("    public string " + h + ";");
                }
                sb.AppendLine("}\n");

                sb.AppendLine("public static class " + wrapperClass);
                sb.AppendLine("{");
                sb.AppendLine("    public static readonly " + className + "[] Items = new " + className + "[]");
                sb.AppendLine("    {");
                foreach (var row in rows)
                {
                    sb.Append("        new " + className + " { ");
                    for (int i = 0; i < headers.Count; i++)
                    {
                        sb.Append(headers[i] + " = \"" + row[i].Replace("\"", "") + "\"");
                        if (i < headers.Count - 1) sb.Append(", ");
                    }
                    sb.AppendLine(" },");
                }
                sb.AppendLine("    };\n}");

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, sb.ToString());
                AssetDatabase.Refresh();
                Debug.Log("Static class and model class generated at " + path);
            }
        }
    }
}