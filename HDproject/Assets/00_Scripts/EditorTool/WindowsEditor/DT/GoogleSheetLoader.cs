using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class GoogleSheetLoader
{
    private const string PrefKeyURLs = "GoogleDTEditor_SheetURLs";
    private const string PrefKeyNames = "GoogleDTEditor_SheetNames";



    // 에디터Prefs에서 시트 목록 불러오기
    public static void LoadPrefs(List<string> sheetURLs, List<string> sheetNames)
    {
        sheetURLs.Clear();
        sheetNames.Clear();
        string urlStr = EditorPrefs.GetString(PrefKeyURLs, "");
        string nameStr = EditorPrefs.GetString(PrefKeyNames, "");
        if (!string.IsNullOrEmpty(urlStr))
            sheetURLs.AddRange(urlStr.Split('|'));
        if (!string.IsNullOrEmpty(nameStr))
            sheetNames.AddRange(nameStr.Split('|'));
        while (sheetNames.Count < sheetURLs.Count)
            sheetNames.Add("");
    }

    // 에디터Prefs에 시트 목록 저장
    public static void SavePrefs(List<string> sheetURLs, List<string> sheetNames)
    {
        EditorPrefs.SetString(PrefKeyURLs, string.Join("|", sheetURLs.ToArray()));
        EditorPrefs.SetString(PrefKeyNames, string.Join("|", sheetNames.ToArray()));
    }

    public static List<List<string>> LoadCSVFromUrl(string csvUrl)
    {
        List<List<string>> result = new List<List<string>>();
        try
        {
            using (WebClient client = new WebClient())
            {
                string data = client.DownloadString(csvUrl);
                using (StringReader reader = new StringReader(data))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(ParseCsvLine(line));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("CSV 로딩 실패: " + e.Message);
        }
        return result;
    }


    // CSV 한 줄을 안전하게 파싱하는 함수
    public static List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Length = 0;
            }
            else
            {
                sb.Append(c);
            }
        }
        result.Add(sb.ToString());
        return result;
    }


    // 자동 코드 생성
    public static void GenerateAllData(List<string> sheetURLs, List<string> sheetNames, string generatedDir = "Assets/Scripts/Generated/")
    {
        for (int idx = 0; idx < sheetURLs.Count; idx++)
        {
            string url = sheetURLs[idx];
            string sheetName = sheetNames[idx];
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(sheetName))
                continue;

            string id = GoogleSheetURLParser.ExtractSpreadsheetId(url);
            string gidStr = GoogleSheetURLParser.ExtractGid(url);

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(gidStr))
            {
                string csvUrl = $"https://docs.google.com/spreadsheets/d/{id}/gviz/tq?tqx=out:csv&gid={gidStr}";
                List<List<string>> tableData = LoadCSVFromUrl(csvUrl);

                if (tableData != null && tableData.Count > 0)
                {
                    List<string> headers = new List<string>();
                    for (int i = 0; i < tableData[0].Count; i++)
                        headers.Add(Regex.Replace(tableData[0][i], "[^a-zA-Z0-9_]", "_"));
                    List<List<string>> rows = tableData.GetRange(1, tableData.Count - 1);

                    // 타입 자동 추론
                    List<string> types = FieldTypeAutoDetect.InferFieldTypes(rows, headers);

                    // 제네릭 코드 생성 호출
                    GenericSheetClassGenerator.Generate(
                        sheetName.Replace(" ", ""),
                        headers,
                        types,
                        rows,
                        generatedDir
                    );
                }
            }
        }

        // DataDirectory.cs 생성
        DataDirectoryGenerator.Generate(generatedDir);

        AssetDatabase.Refresh();
        Debug.Log("모든 데이터 클래스 + DataDirectory 생성 완료");
    }

}

