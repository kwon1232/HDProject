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
    private List<string> sheetURLs = new List<string>();
    private List<string> sheetTabNames = new List<string>();
    private List<string> classNames = new List<string>();
    private string searchString = "";
    private Vector2 scroll;

    [MenuItem("Tools/Google/DT Editor")]
    public static void Open()
    {
        GetWindow<GoogleDTEditor>("Google DT Editor");
    }

    private void OnEnable()
    {
        GoogleSheetLoader.LoadPrefs(sheetURLs, sheetTabNames, classNames);
    }

    private void OnDisable()
    {
        GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Google 시트 주소 & 클래스명 관리", EditorStyles.boldLabel);

        // 시트 목록
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(250));
        for (int i = 0; i < sheetURLs.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            sheetURLs[i] = EditorGUILayout.TextField("Sheet URL", sheetURLs[i]);
            sheetTabNames[i] = EditorGUILayout.DelayedTextField("Sheet Name", sheetTabNames[i]);
            classNames[i] = EditorGUILayout.TextField("Class Name", classNames[i]);
            bool toDelete = GUILayout.Button("Delete");
            EditorGUILayout.EndVertical();

            if (toDelete)
            {
                sheetURLs.RemoveAt(i);
                sheetTabNames.RemoveAt(i);
                classNames.RemoveAt(i);
                GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
                break;
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Sheet"))
        {
            sheetURLs.Add("");
            sheetTabNames.Add("");
            classNames.Add("");
            GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate All Data Classes"))
        {
            GoogleSheetLoader.GenerateAllData(sheetURLs, sheetTabNames, classNames);
            GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
        }

        if (GUILayout.Button("Update Data"))
        {
            GoogleSheetLoader.GenerateAllData(sheetURLs, sheetTabNames, classNames);
            GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset"))
        {
            sheetURLs.Clear();
            sheetTabNames.Clear();
            classNames.Clear();
            GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
        }
    }
}
