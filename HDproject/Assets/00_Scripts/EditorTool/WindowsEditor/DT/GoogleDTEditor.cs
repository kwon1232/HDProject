using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

using System.Text.RegularExpressions;// 정규 표현식을 사용하기 위한 네임스페이스

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
    // StringBuilder 사용 이유
    // StringBuilder는 문자열을 자주 덮어쓰거나 조작할 때 GC 부담을 줄이고 성능 향상에 도움
    private StringBuilder sheetURL = new StringBuilder();
    private StringBuilder sheetName = new StringBuilder();
    private StringBuilder spreadSheetId = new StringBuilder();
    private StringBuilder gid = new StringBuilder();

    private List<List<string>> tableData;
    private Vector2 scroll;

    [MenuItem("Tools/Google/DT Editor")]
    public static void Open()
    {
        GetWindow<GoogleDTEditor>("Google DT Editor");
    }

    private void OnGUI()
    {
        string currentURLInput = EditorGUILayout.TextField("Google Sheet URL", sheetURL.ToString());
        if (!currentURLInput.Equals(sheetURL.ToString()))
        {
            sheetURL.Clear().Append(currentURLInput);
        }

        if (GUILayout.Button("Load Sheet"))
        {
            string id = GoogleSheetURLParser.ExtractSpreadsheetId(sheetURL.ToString());
            string gidStr = GoogleSheetURLParser.ExtractGid(sheetURL.ToString());

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(gidStr))
            {
                spreadSheetId.Clear().Append(id);
                gid.Clear().Append(gidStr);
                // Sheet data will be loaded using GoogleSheetLoader
            }
        }
    }

}
